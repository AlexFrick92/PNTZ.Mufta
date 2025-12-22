using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Timers;
using Promatis.Core.Threading;
using Timer = System.Timers.Timer;

namespace Promatis.Core.IO
{
    /// <summary>
    /// Класс обертка над <see cref="FileSystemEventArgs"/> и <see cref="RenamedEventArgs"/>.
    /// </summary>
    internal class DelayedEvent
    {
        public DelayedEvent(FileSystemEventArgs args)
        {
            Args = args;
        }

        public FileSystemEventArgs Args { get; }

        public bool Delayed { get; set; }

        public virtual bool IsDuplicate(object obj)
        {
            if (!(obj is DelayedEvent delayedEvent))
            {
                return false; // this is not null so they are different 
            }

            FileSystemEventArgs eO1 = Args;
            RenamedEventArgs reO1 = Args as RenamedEventArgs;
            FileSystemEventArgs eO2 = delayedEvent.Args;
            RenamedEventArgs reO2 = delayedEvent.Args as RenamedEventArgs;
            // The events are equal only if they are of the same type (reO1 and reO2
            // are both null or NOT NULL) and have all properties equal.         
            // We also eliminate Changed events that follow recent Created events
            // because many apps create new files by creating an empty file and then 
            // they update the file with the file content.
            return eO1 != null && eO2 != null && eO1.ChangeType == eO2.ChangeType
                   && eO1.FullPath == eO2.FullPath && eO1.Name == eO2.Name &&
                   (reO1 == null & reO2 == null || reO1 != null && reO2 != null &&
                    reO1.OldFullPath == reO2.OldFullPath && reO1.OldName == reO2.OldName) ||
                   eO1 != null && eO2 != null && eO1.ChangeType == WatcherChangeTypes.Created
                   && eO2.ChangeType == WatcherChangeTypes.Changed
                   && eO1.FullPath == eO2.FullPath && eO1.Name == eO2.Name;
        }
    }

    /// <summary>
    /// Отслеживает все события файловой системы по указанному пути и задерживает их вызов на то количество миллисекунд,
    /// которое указано в свойстве <seealso cref="ConsolidationInterval"/> (по умолчанию 1000) для фильтрации повторяющихся событий
    /// </summary>
    /// <remarks>При обнаружении дублирующихся событий сработает только первое, остальные будут игнорированы.</remarks>
    public class DelayedFileSystemWatcher : IDisposable
    {
        private readonly FileSystemWatcher _fileSystemWatcher;

        // Lock order is _enterThread, _events.SyncRoot
        private readonly object _enterThread = new object(); // Only one timer event is processed at any given moment
        private ArrayList _events;

        private Timer _serverTimer;
        private int _msConsolidationInterval = 1000; // milliseconds

        #region Delegate to FileSystemWatcher

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="DelayedFileSystemWatcher"/>
        /// </summary>
        public DelayedFileSystemWatcher()
        {
            _fileSystemWatcher = new FileSystemWatcher();
            Initialize();
        }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="DelayedFileSystemWatcher"/>
        /// </summary>
        /// <param name="path">Путь в файловой системе, по которому будут отслеживаться изменения</param>
        public DelayedFileSystemWatcher(string path)
        {
            _fileSystemWatcher = new FileSystemWatcher(path);
            Initialize();
        }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="DelayedFileSystemWatcher"/>
        /// </summary>
        /// <param name="path">Путь к отслеживаемой папке</param>
        /// <param name="filter">Строка фильтра для указания файлов которые будут отслеживаться в заданой папке</param>
        public DelayedFileSystemWatcher(string path, string filter)
        {
            _fileSystemWatcher = new FileSystemWatcher(path, filter);
            Initialize();
        }

        /// <summary>
        /// Интервал задержки срабатывания событий из <see cref="FileSystemWatcher"/>
        /// </summary>
        public int ConsolidationInterval
        {
            get => _msConsolidationInterval;
            set
            {
                _msConsolidationInterval = value;
                _serverTimer.Interval = value;
            }
        }

        /// <summary>
        /// Признак, показывающий состояние остлеживаемости (включена/отключена)
        /// </summary>
        /// <remarks>По умолчанию <c>false</c>, т.е. отключена</remarks>
        public bool EnableRaisingEvents
        {
            get => _fileSystemWatcher.EnableRaisingEvents;
            set
            {
                _fileSystemWatcher.EnableRaisingEvents = value;
                if (value)
                {
                    _serverTimer.Start();
                }
                else
                {
                    _serverTimer.Stop();
                    _events.Clear();
                }
            }
        }

        /// <summary>
        /// Строка фильтра определяющая какие файлы будут отслеживаться в папке.
        /// </summary>
        /// <remarks>По умолчанию <c>*.*</c>, т.е. отслеживаются все файлы</remarks>
        public string Filter
        {
            get => _fileSystemWatcher.Filter;
            set => _fileSystemWatcher.Filter = value;
        }

        /// <summary>
        /// Признак, указывающий на необходимость отслеживать содержимое подпапок
        /// </summary>
        /// <remarks>По умолчанию <c>false</c>, т.е. не отслеживать</remarks>
        public bool IncludeSubdirectories
        {
            get => _fileSystemWatcher.IncludeSubdirectories;
            set => _fileSystemWatcher.IncludeSubdirectories = value;
        }

        /// <summary>
        /// Размер внутреннего буфера
        /// </summary>
        /// <remarks>По умолчанию 8192 (8K)</remarks>
        public int InternalBufferSize
        {
            get => _fileSystemWatcher.InternalBufferSize;
            set => _fileSystemWatcher.InternalBufferSize = value;
        }

        /// <summary>
        /// Тип отслеживаемых изменений
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>По умолчанию логическое или между <c>LastWrite</c>, <c>FileName</c>, и <c>DirectoryName</c></remarks>
        public NotifyFilters NotifyFilter
        {
            get => _fileSystemWatcher.NotifyFilter;
            set => _fileSystemWatcher.NotifyFilter = value;
        }

        /// <summary>
        /// Путь к отслеживаемой папке
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>По умолчанию <c>""</c>, т.е. текущая папка</remarks>
        public string Path
        {
            get => _fileSystemWatcher.Path;
            set => _fileSystemWatcher.Path = value;
        }

        /// <summary>
        /// Gets or sets the object used to marshal the event handler calls issued as a result of a directory change.
        /// </summary>
        /// <returns>The System.ComponentModel.ISynchronizeInvoke that represents the object used to marshal 
        /// the event handler calls issued as a result of a directory change. The default is null.</returns>
        public ISynchronizeInvoke SynchronizingObject
        {
            get => _fileSystemWatcher.SynchronizingObject;
            set => _fileSystemWatcher.SynchronizingObject = value;
        }

        /// <summary>
        /// Событие, возникающее при изменении папки или файла.
        /// </summary>
        public event FileSystemEventHandler Changed;

        /// <summary>
        /// Событие, возникающее при создании папки или файла.
        /// </summary>
        public event FileSystemEventHandler Created;

        /// <summary>
        /// Событие, возникающее при удалении папки или файла.
        /// </summary>
        public event FileSystemEventHandler Deleted;

        /// <summary>
        /// Событие, возникающее при переполнении внутреннего буфера.
        /// </summary>
        public event ErrorEventHandler Error;

        /// <summary>
        /// Событие, возникающее при переименовании папки или файла.
        /// </summary>
        public event RenamedEventHandler Renamed;

        /// <summary>
        /// Begins the initialization of a System.IO.FileSystemWatcher used on a form or used by another component. The initialization occurs at run time.
        /// </summary>
        public void BeginInit() => _fileSystemWatcher.BeginInit();

        /// <summary>
        /// Ends the initialization of a System.IO.FileSystemWatcher used on a form or used by another component. The initialization occurs at run time.
        /// </summary>
        public void EndInit() => _fileSystemWatcher.EndInit();

        /// <inheritdoc />
        public void Dispose() => Uninitialize();

        /// <summary>
        /// A synchronous method that returns a structure that contains specific information on the change that occurred, given the type of change you want to monitor.
        /// </summary>
        /// <param name="changeType">The System.IO.WatcherChangeTypes to watch for.</param>
        /// <returns>A System.IO.WaitForChangedResult that contains specific information on the change that occurred</returns>
        public WaitForChangedResult WaitForChanged(WatcherChangeTypes changeType)
        {
            return new WaitForChangedResult();
        }

        /// <summary>
        /// A synchronous method that returns a structure that contains specific information on the change that occurred, 
        /// given the type of change you want to monitor and the time (in milliseconds) to wait before timing out.
        /// </summary>
        /// <param name="changeType">The System.IO.WatcherChangeTypes to watch for.</param>
        /// <param name="timeout">The time (in milliseconds) to wait before timing out.</param>
        /// <returns>A System.IO.WaitForChangedResult that contains specific information on the change that occurred.</returns>
        public WaitForChangedResult WaitForChanged(WatcherChangeTypes changeType, int timeout)
        {
            return new WaitForChangedResult();
        }

        #endregion
        #region Implementation
        private void Initialize()
        {
            _events = ArrayList.Synchronized(new ArrayList(32));
            _fileSystemWatcher.Changed += FileSystemEventHandler;
            _fileSystemWatcher.Created += FileSystemEventHandler;
            _fileSystemWatcher.Deleted += FileSystemEventHandler;
            _fileSystemWatcher.Error += ErrorEventHandler;
            _fileSystemWatcher.Renamed += RenamedEventHandler;
            _serverTimer = new Timer(_msConsolidationInterval);
            _serverTimer.Elapsed += ElapsedEventHandler;
            _serverTimer.AutoReset = true;
            _serverTimer.Enabled = _fileSystemWatcher.EnableRaisingEvents;
        }
        private void Uninitialize()
        {
            _fileSystemWatcher?.Dispose();
            _serverTimer?.Dispose();
        }

        private void FileSystemEventHandler(object sender, FileSystemEventArgs e) => _events.Add(new DelayedEvent(e));

        private void ErrorEventHandler(object sender, ErrorEventArgs e) => Error?.Invoke(this, e);

        private void RenamedEventHandler(object sender, RenamedEventArgs e) => _events.Add(new DelayedEvent(e));

        private void ElapsedEventHandler(Object sender, ElapsedEventArgs e)
        {
            // We don't fire the events inside the lock. We will queue them here until
            // the code exits the locks.
            Queue eventsToBeFired = null;
            if (Monitor.TryEnter(_enterThread))
            {
                // Only one thread at a time is processing the events                
                try
                {
                    eventsToBeFired = new Queue(32);
                    // Lock the collection while processing the events
                    WaitLock.Lock(_events.SyncRoot, 10000, () =>
                    {
                        for (int i = 0; i < _events.Count; i++)
                        {
                            if (_events[i] is DelayedEvent current)
                            {
                                if (current.Delayed)
                                {
                                    // This event has been delayed already so we can fire it
                                    // We just need to remove any duplicates
                                    for (int j = i + 1;
                                        j < _events.Count;
                                        j++)
                                    {
                                        if (
                                            current.IsDuplicate(
                                                _events[j]))
                                        {
                                            // Removing later duplicates
                                            _events.RemoveAt(j);
                                            j--; // Don't skip next event
                                        }
                                    }

                                    // Add the event to the list of events to be fired
                                    eventsToBeFired.Enqueue(current);
                                    // Remove it from the current list
                                    _events.RemoveAt(i);
                                    i--; // Don't skip next event
                                }
                                else
                                {
                                    // This event was not delayed yet, so we will delay processing
                                    // this event for at least one timer interval
                                    current.Delayed = true;
                                }
                            }
                        }
                    });
                }
                finally
                {
                    Monitor.Exit(_enterThread);
                }
            }
            // else - this timer event was skipped, processing will happen during the next timer event

            // Now fire all the events if any events are in eventsToBeFired
            RaiseEvents(eventsToBeFired);
        }

        /// <summary>
        /// Вызывает последовательно события из заданной очереди
        /// </summary>
        /// <param name="deQueue">Очередь событий</param>
        protected void RaiseEvents(Queue deQueue)
        {
            if (deQueue != null && deQueue.Count > 0)
            {
                while (deQueue.Count > 0)
                {
                    if (deQueue.Dequeue() is DelayedEvent de)
                        switch (de.Args.ChangeType)
                        {
                            case WatcherChangeTypes.Changed:
                                Changed?.Invoke(this, de.Args);
                                break;
                            case WatcherChangeTypes.Created:
                                Created?.Invoke(this, de.Args);
                                break;
                            case WatcherChangeTypes.Deleted:
                                Deleted?.Invoke(this, de.Args);
                                break;
                            case WatcherChangeTypes.Renamed:
                                Renamed?.Invoke(this, de.Args as RenamedEventArgs);
                                break;
                        }
                }
            }
        }
        #endregion
    }
}

Команды делать от админа
1. Сгенерировать самоподписанный сертификат:

```powershell
New-SelfSignedCertificate -DnsName "ATISSA" -CertStoreLocation "cert:\LocalMachine\My"
```

Теперь его можно найти так: ALT + R "certlm.msc" там в папке "Личное/сертификаты"

2. Назначить на порт этот сертификат. 

Для этого, скопировать из "certlm.msc" отпечаток(THUMBPRINT) во вкладке "Состав" он будет внизу.
```powershell
netsh http add sslcert ipport=0.0.0.0:61778 certhash=THUMBPRINT appid="{12345678-1234-1234-1234-1234567890ab}"
```
THUMBPRINT - подставить отпечаток от сертификата, appid - новый Guid.

команда для отображения назначеных сертификатов:
```powershell
netsh http show sslcert  
```
команда для удаления назначенных на порты сертификатов
```powershell
netsh http delete sslcert ipport=0.0.0.0:61777
```

3. Перезапустить службу АТИССЫ с параметрами, где в адресе будет указан уже https.

Сначала изменим конфиг службы:
```powershell
sc config TMK.VLPP.TRW3.PIMS binPath= "C:\AppServer\TMK\VLPP\TRW3\Promatis.ModuleLauncher.exe C:\AppServer\TMK\VLPP\TRW3\Modules\PIMS /host:https://10.10.10.133:61778/VLPP/TRW3/PIMS /name:PIMS /rabbitHost:amqp://mes:mes@TestServer/VLPP.TRW3 /srv"
```
затем перезапустим службу
```powershell
sc stop TMK.VLPP.TRW3.PIMS
```
```powershell
sc start TMK.VLPP.TRW3.PIMS
```

!!!Про ПОРТЫ!!!

Я пытался запустить на 61777 порте, который у нас сейчас, но OWIN не позволяет запустить одну слжубу на https, а остальные на http. 
Так что переходить нужно либо сразу всеми службами, либо использовать другой порт. 

После этого, можно зайти через браузер и SWAGGER 
```
https://10.10.10.133:61778/VLPP/trw3/pims/swagger/ui/index
```

Подтвердить, что принимаешь сертификат.



На клиенте!!!

Нужно в `profilecatalog.xml` поменять адрес на https с нужным портом.

А так же в webapiProxy.generated.cs
```C#
private HttpClient CreateClient()
{
    var moduleUri = new Uri(Configuration.DCABaseAddress);

    ///ДОПИСАТЬ ЭТУ СТРОЧКУ
    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
    
    var servicePoint = ServicePointManager.FindServicePoint(moduleUri);
    servicePoint.ConnectionLimit = Int32.MaxValue;
    return new HttpClient(new HttpClientHandler { UseCookies = false, Proxy = null, UseProxy = false })
    {
        BaseAddress = moduleUri,
        Timeout = Configuration.Timeout
    };
}

```

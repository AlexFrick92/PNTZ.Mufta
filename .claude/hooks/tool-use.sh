#!/bin/bash

# Напоминание перед добавлением файлов в проект
if [[ "$TOOL_NAME" == "Write" ]]; then
    echo "⚠️  Reminder: Add new files to .csproj (XAML as <Page>, C# as <Compile>)"
fi

# Напоминание перед коммитами
if [[ "$TOOL_NAME" == "Bash" ]] && [[ "$COMMAND" == *"git commit"* ]]; then
    echo "⚠️  Reminder: Use commit format from CLAUDE.md"
fi
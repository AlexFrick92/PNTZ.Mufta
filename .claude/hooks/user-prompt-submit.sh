#!/bin/bash

# Если задача требует анализа большого объёма кода
if [[ "$USER_PROMPT" == *"весь проект"* ]] || [[ "$USER_PROMPT" == *"все файлы"* ]] || [[ "$USER_PROMPT" == *"вся кодовая база"* ]]; then
    echo "💡 Hint: For large codebase analysis consider using /gemini command with Gemini CLI"
fi
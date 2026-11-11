# Database Plugin

Нативный плагин LinCtrl для MySQL. Использует ABI 3, единственный `DatabaseDispatcher` для команд и запросов, официальный драйвер MySQL и `PasswordHashService` из Hosting.

## Сборка и запуск

Расположите репозиторий `bundle` рядом с этим репозиторием. Хост и плагин должны быть собраны из SDK с поддержкой метаданных обработчиков. Для Windows x64 выполните:

```powershell
dotnet publish DatabasePlugin.csproj -c Release -r win-x64
```

Поместите `DatabasePlugin.dll` и `manifest.json` в подкаталог `plugins/database` рядом с хостом. Хост передаёт плагину параметры `LinCtrlDatabase:Host`, `Database`, `Username` и `Password` через `IConfiguration`. Для JWT используются настройки хоста. Схема БД соответствует `Sudzekai.LinCtrl.Hosting/Database/schema.sql`.

При старте создаётся администратор с именем и паролем пользователя MySQL. Пароль хранится в приложении как хэш Argon2id. При конфликте с существующей записью плагин сообщает об ошибке и сохраняет запись без изменений.

## Маршруты

| Маршрут | Viewer и выше | Только Administrator |
| --- | --- | --- |
| `/database/version` | GET | |
| `/database/roles`, `/database/roles/{id}` | GET | |
| `/database/users`, `/database/users/{id}` | GET | POST для коллекции, PUT и DELETE по ID |
| `/database/history`, `/database/history/{id}` | GET | POST для коллекции, PUT и DELETE по ID |

Все маршруты принимают JWT хоста. Администратор не может менять или удалять собственную учётную запись. Списки пользователей и истории принимают `limit` от 1 до 200 (по умолчанию 50) и `offset` (по умолчанию 0).

Для записи пользователя используются поля `username`, `password`, `roleId` и `isBlocked`. При создании пароль обязателен, признак блокировки по умолчанию равен `false`. При обновлении пропущенные пароль и признак блокировки сохраняют текущие значения. В ответах о пользователях доступны только `id` и `username`.

Для записи истории используются поля `userId`, `dateTime`, `action` и `summary`. Если `dateTime` пропущено, при создании используется текущее время UTC, при обновлении сохраняется время записи.

Все обработчики включают `IsFiltering`. Строка версии приходит в `data`, например `{"success":true,"data":"0.2.0"}`.

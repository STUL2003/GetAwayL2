# Тестовое задание
# TCP-шлюз "камера - ПЛК"
_______________________________________________
**Инструкция по запуску**
.NET10, ASP.NET, PostgreSQL<br>
* Создание таблицы:
```sh
CREATE DATABASE EquipDBLogs
#Можно попробовать восстановить бэкап:
pg_restore -h localhost -U dima -d EquipmentMonitor -v equipment.backup
```
* Добавить в appsetings.json конфиг подключения:
```
	"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=EquipDBLogs;Username=postgres;Password=1234"}
```
* Выполнить миграцию:
```
  dotnet ef migrations add InitialCreate
	dotnet ef database update
```
* Запустить эмулятор ПЛК
```
  dotnet run --project TCPPlk
```
* Запустить ШЛЮЗ
```
dotnet run
```
* Запросы
```
curl -X POST http://localhost:5187/api/Camera/cam_msg -H "Content-Type: application/json" -d "{\"msg\":\"{сообщение от камеры}\"}"
curl -X GET http://localhost:5187/api/logs

```

--create login poem with password='Brent532Beat'

create user poem_user from login poem
go

exec sp_addrolemember 'db_owner', 'poem_user'
go

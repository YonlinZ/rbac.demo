本地数据库账号：sa
本地数据库密码：password1

注册用户：
{
	"username":"username1",
	"password":"password1-A",
	"email":"email1@email.com"
}


添加迁移：Add-Migration <迁移名称>
将迁移应用到数据库：Update-Database
删除迁移文件：Remove-Migration

将所有的迁移生成一个sql脚本，用于部署：Script-Migration
-- Add "User Groups" menu item under the existing "Users" parent menu (SQL Server)
IF NOT EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'User Groups' AND [Url] = '/user-groups')
BEGIN
    DECLARE @parentId UNIQUEIDENTIFIER;
    DECLARE @nextOrder INT;

    SELECT @parentId = [Id] FROM [appshell].[MenuItem] WHERE [Name] = 'Users';

    IF @parentId IS NOT NULL
    BEGIN
        SELECT @nextOrder = COALESCE(MAX([MenuOrder]), 0) + 1
        FROM [appshell].[MenuItem]
        WHERE [ParentId] = @parentId;

        INSERT INTO [appshell].[MenuItem] ([Id], [Name], [Url], [ParentId], [MenuOrder])
        VALUES (NEWID(), 'User Groups', '/user-groups', @parentId, @nextOrder);
    END
END

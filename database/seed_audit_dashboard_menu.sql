-- Seed: Add "Audit Dashboard" menu item under System Management
-- Run against SMPortal database

-- Add Audit Dashboard under System Management
IF NOT EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'Audit Dashboard')
BEGIN
    DECLARE @systemMgmtId UNIQUEIDENTIFIER;
    SELECT @systemMgmtId = [Id] FROM [appshell].[MenuItem] WHERE [Name] = 'System Management' AND [ParentId] IS NULL;

    IF @systemMgmtId IS NOT NULL
    BEGIN
        INSERT INTO [appshell].[MenuItem] ([Id], [Name], [Url], [ParentId], [MenuOrder])
        VALUES (NEWID(), 'Audit Dashboard', '/audit-dashboard', @systemMgmtId, 1);

        -- Update Audit Logs order to 2 if it exists
        UPDATE [appshell].[MenuItem]
        SET [MenuOrder] = 2
        WHERE [Name] = 'Audit Logs' AND [ParentId] = @systemMgmtId AND [MenuOrder] = 1;

        PRINT 'Audit Dashboard menu item added under System Management.';
    END
    ELSE
    BEGIN
        PRINT 'WARNING: System Management parent menu not found.';
    END
END
ELSE
BEGIN
    PRINT 'Audit Dashboard menu item already exists.';
END
GO

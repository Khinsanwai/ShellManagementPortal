-- Remove "User App Assignment" menu item from sidebar
-- Run this against the SMPortal database

DELETE FROM [appshell].[MenuItem]
WHERE [Name] = 'User App Assignment'
   OR [Url] = '/user-app-assign';

PRINT 'User App Assignment menu item removed.';

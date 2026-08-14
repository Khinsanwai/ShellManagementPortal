-- Add API Resources menu item
-- Run this script to add the API Resources menu to the Shell Management Portal

-- First, find the parent menu ID for 'Application Management' or 'Authorization Management'
-- Adjust the parent ID based on your menu structure

-- Insert API Resources as a top-level menu item (adjust ParentId as needed)
INSERT INTO [appshell].[MenuItem] (
    [Id],
    [Name],
    [Url],
    [Icon],
    [MenuOrder],
    [IsActive],
    [ParentId],
    [CreatedDate]
)
VALUES (
    NEWID(),
    'API Resources',
    '/api-resources',
    'api',
    20,  -- Adjust order as needed
    1,   -- Active
    NULL, -- Top-level menu (set ParentId if you want it under a parent menu)
    GETDATE()
);

-- If you want to assign this menu to admin users, you can use:
-- INSERT INTO [appshell].[UserMenuAssignment] (...)
-- SELECT ... FROM [appshell].[MenuItem] WHERE Name = 'API Resources'

PRINT 'API Resources menu item added successfully.';

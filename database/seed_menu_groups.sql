-- Add "User Groups" menu item under the existing "Users" parent menu
INSERT INTO `appshell`.`MenuItem` (`Id`, `Name`, `Url`, `ParentId`, `MenuOrder`)
SELECT 
    UUID(), 
    'User Groups', 
    '/groups', 
    `Id`, 
    (SELECT COALESCE(MAX(`MenuOrder`), 0) + 1 FROM `appshell`.`MenuItem` WHERE `ParentId` = `parent`.`Id`)
FROM `appshell`.`MenuItem` `parent`
WHERE `parent`.`Name` = 'Users'
  AND NOT EXISTS (SELECT 1 FROM `appshell`.`MenuItem` WHERE `Name` = 'User Groups' AND `Url` = '/groups');

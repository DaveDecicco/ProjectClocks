USE [ProjectClocks]
GO

-- ********************* GROUPS

INSERT INTO [projectclocks].[Groups] ([parent_id],[org_id],[name],[description],[created],[status])
VALUES
-- Highest Level Group (Test) parent group [id = 1]
--(null,null,'Test','Top Level',GETDATE(),1),

-- Top Level Group (Mongrel Consulting) [id = 2]
(0,1,'Mongrel Consulting','Top Level',GETDATE(),1),

-- Children of Top Level Group
-- 1st Level Groups (North Region) [id = 3]
(2,1,'North Region','Orgs in North Region',GETDATE(),1),
-- 1st Level Groups (East Region) [id = 4]
(2,1,'East Region','Orgs in East Region',GETDATE(),1),
-- 1st Level Groups (South Region) [id = 5]
(2,1,'South Region','Orgs in South Region',GETDATE(),1),
-- 1st Level Groups (West Region) [id = 6]
(2,1,'West Region','Orgs in West Region',GETDATE(),1),

-- Children of 1st level groups
-- (North Sub Region 1) [id = 7]
(3,1,'Sub Region 1','North Sub Region 1',GETDATE(),1),
-- (North Sub Region 2) [id = 8]
(3,1,'Sub Region 2','North Sub Region 2',GETDATE(),1)


-- ********************* ROLES
/*
Role rank, an integer value between 0-512. Predefined role ranks:
                                           # User - 4, Supervisor - 12, Client - 16,
                                           # Co-manager - 68, Manager - 324, Top manager - 512.
                                           # Rank is used to determine what "lesser roles" are in each group
                                           # for situations such as "manage_users".
Rights columns values: administer_site,track_own_time,track_own_expenses,view_own_reports,view_own_charts,view_own_projects,view_own_tasks,manage_own_settings,view_users,view_client_reports,view_client_invoices,track_time,track_expenses,view_reports,approve_reports,approve_timesheets,view_charts,view_own_clients,override_punch_mode,override_own_punch_mode,override_date_lock,override_own_date_lock,swap_roles,manage_own_account,manage_users,manage_projects,manage_tasks,manage_custom_fields,manage_clients,manage_invoices,override_allow_ip,manage_basic_settings,view_all_reports,manage_features,manage_advanced_settings,manage_roles,export_data,approve_all_reports,approve_own_timesheets,manage_subgroups,view_client_unapproved,delete_group
*/

INSERT INTO [projectclocks].[Roles] ([group_id],[name],[description],[rank],[rights],[status])
VALUES
-- id=1 
--(0,'Site Administrator',1024,'administer_site',1),
-- id=2
--(0,'Top manager',512,'track_own_time,track_own_expenses,view_own_reports,view_own_charts,view_own_projects,view_own_tasks,manage_own_settings,view_users,view_client_reports,view_client_invoices,track_time,track_expenses,view_reports,approve_reports,approve_timesheets,view_charts,view_own_clients,override_punch_mode,override_own_punch_mode,override_date_lock,override_own_date_lock,swap_roles,manage_own_account,manage_users,manage_projects,manage_tasks,manage_custom_fields,manage_clients,manage_invoices,override_allow_ip,manage_basic_settings,view_all_reports,manage_features,manage_advanced_settings,manage_roles,export_data,approve_all_reports,approve_own_timesheets,manage_subgroups,view_client_unapproved,delete_group',1),
-- id=3
(0,'Co-manager','Co-manager',68,'view_client_reports',1),
-- id=4
(0,'User','User',4,'track_own_time,track_own_expenses,view_own_reports,view_own_charts,view_own_projects,view_own_tasks,manage_own_settings',1),
-- id=5
(0,'Client','Client',16,'view_client_reports,view_client_invoices,track_time,track_expenses,view_reports',1),
-- id=6
(2,'Software Engineer','User',4,'track_own_time,track_own_expenses,view_own_reports,view_own_charts,view_own_projects,view_own_tasks,manage_own_settings',1),
-- id=7
(3,'Database Engineer','User',4,'track_own_time,track_own_expenses,view_own_reports,view_own_charts,view_own_projects,view_own_tasks,manage_own_settings',1),
-- id=8
(2,'Dave Decicco','Top manager',512,'track_own_time,track_own_expenses,view_own_reports,view_own_charts,view_own_projects,view_own_tasks,manage_own_settings,view_users,view_client_reports,view_client_invoices,track_time,track_expenses,view_reports,approve_reports,approve_timesheets,view_charts,view_own_clients,override_punch_mode,override_own_punch_mode,override_date_lock,override_own_date_lock,swap_roles,manage_own_account,manage_users,manage_projects,manage_tasks,manage_custom_fields,manage_clients,manage_invoices,override_allow_ip,manage_basic_settings,view_all_reports,manage_features,manage_advanced_settings,manage_roles,export_data,approve_all_reports,approve_own_timesheets,manage_subgroups,view_client_unapproved,delete_group',1),
-- id=9
(2,'Chris Sekula','Co-manager',68,'view_client_reports',1),
-- id=10
(2,'Solera','Client',16,'view_client_reports,view_client_invoices,track_time,track_expenses,view_reports',1),
-- id=11
(3,'Omnitracs','Client',16,'view_client_reports,view_client_invoices,track_time,track_expenses,view_reports',1),
-- id=12
(6,'Turnpike','Client',16,'view_client_reports,view_client_invoices,track_time,track_expenses,view_reports',1)


-- ********************* USERS

INSERT INTO [projectclocks].[Users] ([login],[password],[name],[group_id],[role_id],[rate],[quota_percent],[email],[created],[status])
VALUES
-- Site Administrator role (role_id = 4) for Mongrel Consulting Group (group_id = 2) [password: secret with an md5 hash]
('ddecicco', HASHBYTES('md5','secret'),'Dave Decicco',2,3,0.00,100.00,'ddecicco@email.com',GETDATE(),1),
-- Client role (rold_id = 5) for North Sub Region 1 (group_id = 7)
('mdecicco', HASHBYTES('md5','secret'),'Mike Decicco',7,5,0.00,100.00,'mdecicco@email.com',GETDATE(),1)


-- ********************* CLIENTS

INSERT INTO [projectclocks].[Clients] ([group_id],[name],[address],[status])
VALUES 
(7,'Mike Decicco', '123 Greenwood Dr.', 1),
(7,'Bob Decicco', '456 Maple Grove Dr.', 1),
(7,'Tony Decicco', '789 Shadowdale Dr.', 1),
(4,'Wilma Moonan-Decicco', '012 Loch Erne Ln.', 1)


-- ********************* PROJECTS
INSERT INTO [projectclocks].[Projects] ([group_id],[name],[description],[status])
VALUES
(7,'TC Energy Deal - Group 7', 'All tasks related to TC Energy Merger',1),
(7,'Oracle Acquisition - Group 7', 'Merger Project',1),
(4,'Group 4 Deal', 'Sample Data',1),
(9,'Group 9 Deal', 'Sample Data',1)


-- ********************* TASKS
INSERT INTO [projectclocks].[Tasks] ([group_id],[name],[description],[status])
VALUES
(7,'Research', 'TC Energy Deal environment research', 1),
(7,'Expenses', 'Oracle research', 1),
(9,'Sales', 'TC Energy Deal environment research', 1),
(4,'Products', 'TC Energy Deal environment research', 1)

-- Populate ProjectTasks table to create projects and corresponding tasks


-- ********************* INVOICES

INSERT INTO [projectclocks].[Invoices] ([group_id],[name],[date],[client_id],[status])
VALUES
-- Client Mikes's [client_id=1] invoice
(7,'Environmental Research for TC',GETDATE(),1,1),
-- Client Mikes's [client_id=1] invoice
(7,'Phone calls',GETDATE(),1,1),
-- Client Tony's [client_id=3] invoice
(3,'Oracle research',GETDATE(),3,1),
-- Client Wilma's [client_id = 4] invoice
(7,'Legal papers filed',GETDATE(),4,1)





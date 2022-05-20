# Usage: 
# 1) Create a database using the "CREATE DATABASE" mysql command.
# 2) Then, execute this script from command prompt with a command like this:
# mysql -h host -u user -p -D db_name < mysql.sql

# create database ProjectClocks character set = 'utf8mb4';

# use ProjectClocks;


#
# Structure for table Groups. A group is a unit of users for whom we are tracking work time.
# This table stores settings common to all group members such as language, week start day, etc.
#
CREATE TABLE `Groups` (
  `id` int NOT NULL auto_increment,                  	 # group id
  `parent_id` int default NULL,                      	 # parent group id
  `org_id` int default NULL,                         	 # organization id (id of top group)
  `group_key` varchar(32) default NULL,                  # group key
  `name` varchar(80) default NULL,                       # group name
  `description` varchar(255) default NULL,               # group description
  `currency` varchar(7) default NULL,                    # currency symbol
  `decimal_mark` char(1) NOT NULL default '.',           # separator in decimals
  `lang` varchar(10) NOT NULL default 'en',              # language
  `date_format` varchar(20) NOT NULL default '%Y-%m-%d', # date format
  `time_format` varchar(20) NOT NULL default '%H:%M',    # time format
  `week_start` smallint(2) NOT NULL default 0,           # Week start day, 0 == Sunday.
  `tracking_mode` smallint(2) NOT NULL default 1,        # tracking mode ("time", "projects" or "projects and tasks")
  `project_required` smallint(2) NOT NULL default 0,     # whether a project selection is required or optional
  `record_type` smallint(2) NOT NULL default 0,          # time record type ("start and finish", "duration", or both)
  `bcc_email` varchar(100) default NULL,                 # bcc email to copy all reports to
  `allow_ip` varchar(255) default NULL,                  # specification from where users are allowed access
  `password_complexity` varchar(64) default NULL,        # password example that defines required complexity
  `plugins` varchar(255) default NULL,                   # a list of enabled plugins for group
  `lock_spec` varchar(255) default NULL,                 # Cron specification for record locking,
                                                         # for example: "0 10 * * 1" for "weekly on Mon at 10:00".
  `holidays` text default NULL,                          # holidays specification
  `workday_minutes` smallint(4) default 480,             # number of work minutes in a regular working day
  `custom_logo` tinyint(4) default 0,                    # whether to use a custom logo or not
  `config` text default NULL,                            # miscellaneous group configuration settings
  `custom_css` text default NULL,                        # custom css for group
  `created` datetime default NULL,                       # creation timestamp
  `created_ip` varchar(45) default NULL,                 # creator ip
  `created_by` int default NULL,                     	 # creator user_id
  `modified` datetime default NULL,                      # modification timestamp
  `modified_ip` varchar(45) default NULL,                # modifier ip
  `modified_by` int default NULL,                    	 # modifier user_id
  `entities_modified` datetime default NULL,             # modification timestamp of group entities (clients, projects, etc.)
  `status` tinyint(4) default 1,                         # group status
  PRIMARY KEY (`id`)
);


#
# Structure for table Roles. This table stores group roles.
#
CREATE TABLE `Roles` (
  `id` int NOT NULL auto_increment,    # Role id. Identifies roles for all groups on the server.
  `group_id` int NOT NULL,             # Group id the role is defined for.
  `org_id` int default NULL,           # Organization id.
  `name` varchar(80) default NULL,         # Role name - custom role name. In case we are editing a
                                           # predefined role (USER, etc.), we can rename the role here.
  `description` varchar(255) default NULL, # Role description.
  `rank` int default 0,                # Role rank, an integer value between 0-512. Predefined role ranks:
                                           # User - 4, Supervisor - 12, Client - 16,
                                           # Co-manager - 68, Manager - 324, Top manager - 512.
                                           # Rank is used to determine what "lesser roles" are in each group
                                           # for situations such as "manage_users".
  `rights` text default NULL,              # Comma-separated list of rights assigned to a role.
                                           # NULL here for predefined roles (4, 16, 68, 324 - manager)
                                           # means a hard-coded set of default access rights.
  `status` tinyint(4) default 1,           # Role status.
  PRIMARY KEY  (`id`)
);

# Create an index that guarantees unique active and inactive role ranks in each group.
create unique index role_idx on Roles(group_id, `rank`, status);

# Insert site-wide roles - site administrator and top manager.
INSERT INTO `Roles` (`group_id`, `name`, `rank`, `rights`) VALUES (0, 'Site administrator', 1024, 'administer_site');
INSERT INTO `Roles` (`group_id`, `name`, `rank`, `rights`) VALUES (0, 'Top manager', 512, 'track_own_time,track_own_expenses,view_own_reports,view_own_charts,view_own_projects,view_own_tasks,manage_own_settings,view_users,view_client_reports,view_client_invoices,track_time,track_expenses,view_reports,approve_reports,approve_timesheets,view_charts,view_own_clients,override_punch_mode,override_own_punch_mode,override_date_lock,override_own_date_lock,swap_roles,manage_own_account,manage_users,manage_projects,manage_tasks,manage_custom_fields,manage_clients,manage_invoices,override_allow_ip,manage_basic_settings,view_all_reports,manage_features,manage_advanced_settings,manage_roles,export_data,approve_all_reports,approve_own_timesheets,manage_subgroups,view_client_unapproved,delete_group');


#
# Structure for table Users. This table is used to store user properties.
#
CREATE TABLE `Users` (
  `id` int NOT NULL auto_increment,            	   # user id
  `login` varchar(50) NOT NULL,					   # user login
  `password` varchar(50) default NULL,             # password hash
  `name` varchar(100) default NULL,                # user name
  `group_id` int NOT NULL,                     	   # group id
  `org_id` int default NULL,                   	   # organization id
  `role_id` int default NULL,                      # role id
  `client_id` int default NULL,                    # client id for "client" user role
  `rate` float(6,2) NOT NULL default '0.00',       # default hourly rate
  `quota_percent` float(6,2) NOT NULL default '100.00', # percent of time quota
  `email` varchar(100) default NULL,               # user email
  `created` datetime default NULL,                 # creation timestamp
  `created_ip` varchar(45) default NULL,           # creator ip
  `created_by` int default NULL,               	   # creator user_id (null for self)
  `modified` datetime default NULL,                # modification timestamp
  `modified_ip` varchar(45) default NULL,          # modifier ip
  `modified_by` int default NULL,              	   # modifier user_id
  `accessed` datetime default NULL,                # last access timestamp
  `accessed_ip` varchar(45) default NULL,          # last access ip
  `status` tinyint(4) default 1,                   # user status
  PRIMARY KEY (`id`)
);

# Create an index that guarantees unique active and inactive logins.
create unique index login_idx on Users(login, status);

# Create admin account with password 'secret'. Admin is a superuser who can create groups.
DELETE from `Users` WHERE login = 'admin';
INSERT INTO `Users` (`login`, `password`, `name`, `group_id`, `role_id`) VALUES ('admin', md5('secret'), 'Admin', '0', (select id from Roles where `rank` = 1024));


#
# Structure for table Projects.
#
CREATE TABLE `Projects` (
  `id` int NOT NULL auto_increment,            # project id
  `group_id` int NOT NULL,                     # group id
  `org_id` int default NULL,                   # organization id
  `name` varchar(80) NOT NULL, 				   # project name
  `description` varchar(255) default NULL,         # project description
  `tasks` text default NULL,                       # comma-separated list of task ids associated with this project
  `status` tinyint(4) default 1,                   # project status
  PRIMARY KEY (`id`)
);

# Create an index that guarantees unique active and inactive projects per group.
create unique index project_idx on Projects(group_id, name, status);


#
# Structure for table Tasks.
#
CREATE TABLE `Tasks` (
  `id` int NOT NULL auto_increment,            # task id
  `group_id` int NOT NULL,                     # group id
  `org_id` int default NULL,                   # organization id
  `name` varchar(80) NOT NULL, 				   # task name
  `description` varchar(255) default NULL,         # task description
  `status` tinyint(4) default 1,                   # task status
  PRIMARY KEY (`id`)
);

# Create an index that guarantees unique active and inactive tasks per group.
create unique index task_idx on Tasks(group_id, name, status);


#
# Structure for table UserProjects. This table maps users to assigned projects.
#
CREATE TABLE `UserProjects` (
  `id` int NOT NULL auto_increment, # bind id
  `user_id` int NOT NULL,           # user id
  `project_id` int NOT NULL,        # project id
  `group_id` int default NULL,      # group id
  `org_id` int default NULL,        # organization id
  `rate` float(6,2) default '0.00',     # rate for this user when working on this project
  `status` tinyint(4) default 1,        # bind status
  PRIMARY KEY (`id`)
);

# Create an index that guarantees unique user to project binds.
create unique index bind_idx on UserProjects(user_id, project_id);


#
# Structure for table ProjectTasks. This table maps projects to assigned tasks.
#
CREATE TABLE `ProjectTasks` (
  `project_id` int NOT NULL,        # project id
  `task_id` int NOT NULL,           # task id
  `group_id` int default NULL,      # group id
  `org_id` int default NULL         # organization id
);

# Indexes for ProjectTasks.
create index project_idx on ProjectTasks(project_id);
create index task_idx on ProjectTasks(task_id);
create unique index project_task_idx on ProjectTasks(project_id, task_id);


#
# Structure for table UserTimeEntries. This is the table where time entries for users are stored.
# If you use custom fields, additional info for each record may exist in CustomFieldLogs.
#
CREATE TABLE `UserTimeEntries` (
  `id` bigint NOT NULL auto_increment,             # time record id
  `user_id` int NOT NULL,                      	   # user id
  `group_id` int default NULL,                 	   # group id
  `org_id` int default NULL,                   	   # organization id
  `date` date NOT NULL,                            # date the record is for
  `start` time default NULL,                       # record start time (for example, 09:00)
  `duration` time default NULL,                    # record duration (for example, 1 hour)
  `client_id` int default NULL,                	   # client id
  `project_id` int default NULL,               	   # project id
  `task_id` int default NULL,                      # task id
  `timesheet_id` int default NULL,                 # timesheet id
  `invoice_id` int default NULL,                   # invoice id
  `comment` text,                                  # user provided comment for time record
  `billable` tinyint(4) default 0,                 # whether the record is billable or not
  `approved` tinyint(4) default 0,                 # whether the record is approved
  `paid` tinyint(4) default 0,                     # whether the record is paid
  `created` datetime default NULL,                 # creation timestamp
  `created_ip` varchar(45) default NULL,           # creator ip
  `created_by` int default NULL,                   # creator user_id
  `modified` datetime default NULL,                # modification timestamp
  `modified_ip` varchar(45) default NULL,          # modifier ip
  `modified_by` int default NULL,                  # modifier user_id
  `status` tinyint(4) default 1,                   # time record status
  PRIMARY KEY (`id`)
);

# Create indexes on UserTimeEntries for performance.
create index date_idx on UserTimeEntries(date);
create index user_idx on UserTimeEntries(user_id);
create index group_idx on UserTimeEntries(group_id);
create index client_idx on UserTimeEntries(client_id);
create index invoice_idx on UserTimeEntries(invoice_id);
create index project_idx on UserTimeEntries(project_id);
create index task_idx on UserTimeEntries(task_id);
create index timesheet_idx on UserTimeEntries(timesheet_id);


#
# Structure for table Invoices. Invoices are issued to clients for billable work.
#
CREATE TABLE `Invoices` (
  `id` int NOT NULL auto_increment,            # invoice id
  `group_id` int NOT NULL,                     # group id
  `org_id` int default NULL,                   # organization id
  `name` varchar(80) NOT NULL, 				   # invoice name
  `date` date NOT NULL,                        # invoice date
  `client_id` int NOT NULL,                    # client id
  `status` tinyint(4) default 1,               # invoice status
  PRIMARY KEY (`id`)
);

# Create an index that guarantees unique invoice names per group.
create unique index name_idx on Invoices(group_id, name, status);


#
# Structure for table TmpUserResetRefs. Used for reset password mechanism.
#
CREATE TABLE `TmpUserResetRefs` (
  `created` datetime default NULL,                 # creation timestamp
  `ref` char(32) NOT NULL default '',              # unique reference for user, used in urls
  `user_id` int NOT NULL                       	   # user id
);


#
# Structure for table ReportConfigurations. Favorite reports are pre-configured report configurations.
#
CREATE TABLE `ReportConfigurations` (
  `id` int NOT NULL auto_increment,                  # favorite report id
  `name` varchar(200) NOT NULL,                          # favorite report name
  `user_id` int NOT NULL,                            # user id favorite report belongs to
  `group_id` int default NULL,                       # group id
  `org_id` int default NULL,                         # organization id
  `report_spec` text default NULL,                       # future replacement field for all report settings
  `client_id` int default NULL,                      # client id (if selected)
  `project_id` int default NULL,                     # project id (if selected)
  `task_id` int default NULL,                        # task id (if selected)
  `billable` tinyint(4) default NULL,                    # whether to include billable, not billable, or all records
  `approved` tinyint(4) default NULL,                    # whether to include approved, unapproved, or all records
  `invoice` tinyint(4) default NULL,                     # whether to include invoiced, not invoiced, or all records
  `timesheet` tinyint(4) default NULL,                   # include records with a specific timesheet status, or all records
  `paid_status` tinyint(4) default NULL,                 # whether to include paid, not paid, or all records
  `users` text default NULL,                             # Comma-separated list of user ids. Nothing here means "all" users.
  `period` tinyint(4) default NULL,                      # selected period type for report
  `period_start` date default NULL,                      # period start
  `period_end` date default NULL,                        # period end
  `show_client` tinyint(4) NOT NULL default 0,           # whether to show client column
  `show_invoice` tinyint(4) NOT NULL default 0,          # whether to show invoice column
  `show_paid` tinyint(4) NOT NULL default 0,             # whether to show paid column
  `show_ip` tinyint(4) NOT NULL default 0,               # whether to show ip column
  `show_project` tinyint(4) NOT NULL default 0,          # whether to show project column
  `show_timesheet` tinyint(4) NOT NULL default 0,        # whether to show timesheet column
  `show_start` tinyint(4) NOT NULL default 0,            # whether to show start field
  `show_duration` tinyint(4) NOT NULL default 0,         # whether to show duration field
  `show_cost` tinyint(4) NOT NULL default 0,             # whether to show cost field
  `show_task` tinyint(4) NOT NULL default 0,             # whether to show task column
  `show_end` tinyint(4) NOT NULL default 0,              # whether to show end field
  `show_note` tinyint(4) NOT NULL default 0,             # whether to show note column
  `show_approved` tinyint(4) NOT NULL default 0,         # whether to show approved column
  `show_work_units` tinyint(4) NOT NULL default 0,       # whether to show work units
  `show_totals_only` tinyint(4) NOT NULL default 0,      # whether to show totals only
  `group_by1` varchar(20) default NULL,                  # group by field 1
  `group_by2` varchar(20) default NULL,                  # group by field 2
  `group_by3` varchar(20) default NULL,                  # group by field 3
  `status` tinyint(4) default 1,                         # favorite report status
  PRIMARY KEY (`id`)
);


#
# Structure for table ReportSchedules. It is used to email ReportConfigurations reports on schedule.
#
CREATE TABLE `ReportSchedules` (
  `id` int NOT NULL auto_increment,         # entry id
  `group_id` int NOT NULL,                  # group id
  `org_id` int default NULL,                # organization id
  `cron_spec` varchar(255) NOT NULL,            # cron specification, "0 1 * * *" for "daily at 01:00"
  `last` int default NULL,                  # UNIX timestamp of when job was last run
  `next` int default NULL,                  # UNIX timestamp of when to run next job
  `report_id` int default NULL,             # report id from ReportConfigurations, a report to mail on schedule
  `email` varchar(100) default NULL,            # email to send results to
  `cc` varchar(100) default NULL,               # cc email to send results to
  `subject` varchar(100) default NULL,          # email subject
  `comment` text,                               # user provided comment for notification
  `report_condition` varchar(255) default NULL, # report condition, "count > 0" for sending not empty reports
  `status` tinyint(4) default 1,                # entry status
  PRIMARY KEY (`id`)
);


#
# Structure for table Clients. A client is an entity for whom work is performed and who may be invoiced.
#
CREATE TABLE `Clients` (
  `id` int NOT NULL AUTO_INCREMENT,            # client id
  `group_id` int NOT NULL,                     # group id
  `org_id` int default NULL,                   # organization id
  `name` varchar(80)  NOT NULL, # client name
  `address` varchar(255) default NULL,             # client address
  `tax` float(6,2) default '0.00',                 # applicable tax for this client
  `projects` text default NULL,                    # comma-separated list of project ids assigned to this client
  `status` tinyint(4) default 1,                   # client status
  PRIMARY KEY (`id`)
);

# Create an index that guarantees unique active and inactive clients per group.
create unique index client_name_idx on Clients(group_id, name, status);


#
# Structure for table ClientProjects. This table maps clients to assigned projects.
#
CREATE TABLE `ClientProjects` (
  `client_id` int NOT NULL,                    # client id
  `project_id` int NOT NULL,                   # project id
  `group_id` int default NULL,                 # group id
  `org_id` int default NULL                    # organization id
);

# Indexes for ClientProjects.
create index client_idx on ClientProjects(client_id);
create index project_idx on ClientProjects(project_id);
create unique index client_project_idx on ClientProjects(client_id, project_id);


#
# Structure for table UserConfigurations. This table is used to store configuration info for users.
# For example, last_report_email parameter stores an email for user last report was emailed to.
#
CREATE TABLE `UserConfigurations` (
  `user_id` int NOT NULL,            # user id
  `group_id` int default NULL,       # group id
  `org_id` int default NULL,         # organization id
  `param_name` varchar(32) NOT NULL,     # parameter name
  `param_value` varchar(80) default NULL # parameter value
);

# Create an index that guarantees unique parameter names per user.
create unique index param_idx on UserConfigurations(user_id, param_name);


# Below are the tables used by CustomFields plugin.

#
# Structure for table CustomFields. This table contains definitions of custom fields.
#
CREATE TABLE `CustomFields` (
  `id` int NOT NULL auto_increment,    # custom field id
  `group_id` int NOT NULL,             # group id
  `org_id` int default NULL,           # organization id
  `entity_type` tinyint(4) default 1,      # type of entity custom field is associated with (time, user, project, task, etc.)
  `type` tinyint(4) NOT NULL default 0,    # custom field type (text or dropdown)
  `label` varchar(32) NOT NULL default '', # custom field label
  `required` tinyint(4) default 0,         # whether this custom field is mandatory for time records
  `status` tinyint(4) default 1,           # custom field status
  PRIMARY KEY  (`id`)
);


#
# Structure for table CustomFieldOptions. This table defines options for dropdown custom fields.
#
CREATE TABLE `CustomFieldOptions` (
  `id` int NOT NULL auto_increment,    # option id
  `group_id` int default NULL,         # group id
  `org_id` int default NULL,           # organization id
  `field_id` int NOT NULL,             # custom field id
  `value` varchar(32) NOT NULL default '', # option value
  `status` tinyint(4) default 1,           # option status
  PRIMARY KEY  (`id`)
);


#
# Structure for table CustomFieldLogs.
# This table supplements UserTimeEntries and contains custom field values for records.
#
CREATE TABLE `CustomFieldLogs` (
  `id` bigint NOT NULL auto_increment, # custom field log id
  `group_id` int default NULL,     # group id
  `org_id` int default NULL,       # organization id
  `log_id` bigint NOT NULL,            # id of a record in UserTimeEntries this record corresponds to
  `field_id` int NOT NULL,         # custom field id
  `option_id` int default NULL,    # Option id. Used for dropdown custom fields.
  `value` varchar(255) default NULL,   # Text value. Used for text custom fields.
  `status` tinyint(4) default 1,       # custom field log entry status
  PRIMARY KEY  (`id`)
);

create index log_idx on CustomFieldLogs(log_id);


#
# Structure for table EntityCustomFields.
# This table stores custom field values for entities such as users and projects
# except for "time" entity (and possibly "expense" in future).
# "time" custom fields are kept separately in CustomFieldLogs
# because UserTimeEntries (and CustomFieldLogs) can grow very large.
#
CREATE TABLE `EntityCustomFields` (
  `id` int(10) unsigned NOT NULL auto_increment, # record id in this table
  `group_id` int(10) unsigned NOT NULL,          # group id
  `org_id` int(10) unsigned NOT NULL,            # organization id
  `entity_type` tinyint(4) NOT NULL,             # entity type
  `entity_id` int(10) unsigned NOT NULL,         # entity id this record corresponds to
  `field_id` int(10) unsigned NOT NULL,          # custom field id
  `option_id` int(10) unsigned default NULL,     # Option id. Used for dropdown custom fields.
  `value` varchar(255) default NULL,             # Text value. Used for text custom fields.
  `created` datetime default NULL,               # creation timestamp
  `created_ip` varchar(45) default NULL,         # creator ip
  `created_by` int(10) unsigned default NULL,    # creator user_id
  `modified` datetime default NULL,              # modification timestamp
  `modified_ip` varchar(45) default NULL,        # modifier ip
  `modified_by` int(10) unsigned default NULL,   # modifier user_id
  `status` tinyint(4) default 1,                 # record status
  PRIMARY KEY  (`id`)
);

# Create an index that guarantees unique custom fields per entity.
create unique index entity_idx on EntityCustomFields(entity_type, entity_id, field_id);


#
# Structure for table ExpenseItems.
# This table lists expense items.
#
CREATE TABLE `ExpenseItems` (
  `id` bigint NOT NULL auto_increment,    # expense item id
  `date` date NOT NULL,                   # date the record is for
  `user_id` int NOT NULL,             # user id the expense item is reported by
  `group_id` int default NULL,        # group id
  `org_id` int default NULL,          # organization id
  `client_id` int default NULL,       # client id
  `project_id` int default NULL,      # project id
  `name` text NOT NULL,                   # expense item name (what is an expense for)
  `cost` decimal(10,2) default '0.00',    # item cost (including taxes, etc.)
  `invoice_id` int default NULL,      # invoice id
  `approved` tinyint(4) default 0,        # whether the item is approved
  `paid` tinyint(4) default 0,            # whether the item is paid
  `created` datetime default NULL,        # creation timestamp
  `created_ip` varchar(45) default NULL,  # creator ip
  `created_by` int default NULL,      # creator user_id
  `modified` datetime default NULL,       # modification timestamp
  `modified_ip` varchar(45) default NULL, # modifier ip
  `modified_by` int default NULL,     # modifier user_id
  `status` tinyint(4) default 1,          # item status
  PRIMARY KEY  (`id`)
);

# Create indexes on ExpenseItems for performance.
create index date_idx on ExpenseItems(date);
create index user_idx on ExpenseItems(user_id);
create index group_idx on ExpenseItems(group_id);
create index client_idx on ExpenseItems(client_id);
create index project_idx on ExpenseItems(project_id);
create index invoice_idx on ExpenseItems(invoice_id);


#
# Structure for table PredefinedExpenses.
# This table keeps names and costs for predefined expenses.
#
CREATE TABLE `PredefinedExpenses` (
  `id` int NOT NULL auto_increment, # predefined expense id
  `group_id` int NOT NULL,          # group id
  `org_id` int default NULL,        # organization id
  `name` varchar(255) NOT NULL,         # predefined expense name, such as mileage
  `cost` decimal(10,2) default '0.00',  # cost for one unit
  PRIMARY KEY  (`id`)
);


#
# Structure for table MonthlyQuotas.
# This table keeps monthly work hour quotas for groups.
#
CREATE TABLE `MonthlyQuotas` (
  `group_id` int NOT NULL,            # group id
  `org_id` int default NULL,          # organization id
  `year` smallint(5) UNSIGNED NOT NULL,   # quota year
  `month` tinyint(3) UNSIGNED NOT NULL,   # quota month
  `minutes` int default NULL,         # quota in minutes in specified month and year
  PRIMARY KEY (`group_id`,`year`,`month`)
);


#
# Structure for table Timesheets. This table keeps timesheet related information.
#
CREATE TABLE `Timesheets` (
  `id` int NOT NULL auto_increment,            # timesheet id
  `user_id` int NOT NULL,                      # user id
  `group_id` int default NULL,                 # group id
  `org_id` int default NULL,                   # organization id
  `client_id` int default NULL,                # client id
  `project_id` int default NULL,               # project id
  `name` varchar(80)  NOT NULL, 			   # timesheet name
  `comment` text,                                  # timesheet comment
  `start_date` date NOT NULL,                      # timesheet start date
  `end_date` date NOT NULL,                        # timesheet end date
  `submit_status` tinyint(4) default NULL,         # submit status
  `approve_status` tinyint(4) default NULL,        # approve status
  `approve_comment` text,                          # approve comment
  `created` datetime default NULL,                 # creation timestamp
  `created_ip` varchar(45) default NULL,           # creator ip
  `created_by` int default NULL,               # creator user_id
  `modified` datetime default NULL,                # modification timestamp
  `modified_ip` varchar(45) default NULL,          # modifier ip
  `modified_by` int default NULL,              # modifier user_id
  `status` tinyint(4) default 1,                   # timesheet status
  PRIMARY KEY (`id`)
);


#
# Structure for table GroupTemplates.
# This table keeps templates used in groups.
#
CREATE TABLE `GroupTemplates` (
  `id` int NOT NULL auto_increment,   # template id
  `group_id` int default NULL,        # group id
  `org_id` int default NULL,          # organization id
  `name` varchar(80)  NOT NULL, # template name
  `description` varchar(255) default NULL,         # template description
  `content` text,                         # template content
  `created` datetime default NULL,        # creation timestamp
  `created_ip` varchar(45) default NULL,  # creator ip
  `created_by` int default NULL,      # creator user_id
  `modified` datetime default NULL,       # modification timestamp
  `modified_ip` varchar(45) default NULL, # modifier ip
  `modified_by` int default NULL,     # modifier user_id
  `status` tinyint(4) default 1,          # template status
  PRIMARY KEY  (`id`)
);


#
# Structure for table ProjectTemplates. This table maps projects to templates.
#
CREATE TABLE `ProjectTemplates` (
  `project_id` int(10) unsigned NOT NULL,        # project id
  `template_id` int(10) unsigned NOT NULL,       # template id
  `group_id` int(10) unsigned NOT NULL,          # group id
  `org_id` int(10) unsigned NOT NULL             # organization id
);

# Indexes for ProjectTemplates.
create index project_idx on ProjectTemplates(project_id);
create index template_idx on ProjectTemplates(template_id);
create unique index project_template_idx on ProjectTemplates(project_id, template_id);


#
# Structure for table FileAttachments.
# This table keeps file attachment information.
#
CREATE TABLE `FileAttachments` (
  `id` int(10) unsigned NOT NULL auto_increment, # file id
  `group_id` int(10) unsigned,                   # group id
  `org_id` int(10) unsigned,                     # organization id
  `remote_id` bigint(20) unsigned,               # file id in storage facility
  `file_key` varchar(32),                        # file key
  `entity_type` varchar(32),                     # type of entity file is associated with (project, task, etc.)
  `entity_id` int(10) unsigned,                  # entity id
  `file_name` varchar(80)  NOT NULL, # file name
  `description` varchar(255) default NULL,       # file description
  `created` datetime default NULL,               # creation timestamp
  `created_ip` varchar(45) default NULL,         # creator ip
  `created_by` int(10) unsigned,                 # creator user_id
  `modified` datetime default NULL,              # modification timestamp
  `modified_ip` varchar(45) default NULL,        # modifier ip
  `modified_by` int(10) unsigned,                # modifier user_id
  `status` tinyint(1) default 1,                 # file status
  PRIMARY KEY  (`id`)
);
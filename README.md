# D365ActivityMigration
When importing or migrating data to a Dynamics 365 / CRM environment you cannot migrate the fields Created By, Modified By and Modified On. Most customers won’t bother about it until it comes down to the migration of ‘old’ activities. By default, the Social Panes show activities sorted on the Modified On date. So when you migrate data and all activities have ‘today’ as the Modified On date, you’ll get problems with the sorting of activities. https://www.dynamics365blog.nl/2017/05/08/migrating-importing-data-and-maintaining-created-by-modified-by-and-modified-on/

# Migrating the data and setting the fields
As you know, it’s not possible to set the values of the Created By, Modified By and Modified On date. That is when you try to set this data directly. Lucky for us, when using the SDK it is actually possible to set the values of those fields. The solution contains a plugin that does this:

At the creation of a record, check if the field ‘dnbs_overriddencreatedby’ contains a value. If so, set the field createdby to that value.
At updating a record, check if the field ‘dnbs_overriddenmodifiedby’ contains a value. If so, set the field modifiedby to that value.
At updating a record, check if the field ‘dnbs_overriddenmodifiedon’ contains a value. If so, set the field modifiedon to that value.
In order to make this work for your data migration or data import, just make sure you set the values of the fields ‘dnbs_overriddencreatedby’, ‘dnbs_overriddenmodifiedby’ and ‘dnbs_overriddenmodifiedon’. These are the schema names of the fields. The display names are: Overridden Created By, Overridden Modified By and Overridden Modified On. When you import the solution, these fields are available on these entities:

- Appointment
- E-mail
- Letter
- Phonecall
- Taks

# The solution is extensible
The plugin inside the solution is setup to fire off on all entities. This means that you can extend the use of this plugin to all other entities. When you want to use the same method on the Account entity, for instance. You just need to make sure you create these fields on that entity. The schema names of those fields must exactly match these names.

dnbs_overriddencreatedby as a Lookup to the User entity;
dnbs_overriddenmodifiedby as a Lookup to the User entity;
dnbs_overriddenmodifiedon as a Date and Time field.

# Don’t forget to delete the solution!
When you are done with migrating or importing your data, don’t forget to delete the solution! As you have seen above, the plugin fires off at all updates. This is a loss to your performance and also you might get into trouble with the Modified By and Modified On fields not being updated right.

# Download your solution here
https://www.dynamics365blog.nl/2017/05/08/migrating-importing-data-and-maintaining-created-by-modified-by-and-modified-on/

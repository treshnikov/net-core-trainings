# Jogging app
## Requirements
The requirements for the test project are: Write a REST API that tracks jogging times of users.

 - API Users must be able to create an account and log in.
 - All API calls must be authenticated.
 - Implement at least three roles with different permission levels: a regular user would only be able to CRUD on their owned records, a user manager would be able to CRUD only users, and an admin would be able to CRUD all records and users.
 - Each time entry when entered has a date, distance, time, and location.
 - Based on the provided date and location, API should connect to a weather API provider and get the weather conditions for the run, and store that with each run.
 - The API must create a report on average speed & distance per week.
 - The API must be able to return data in the JSON format.
 - The API should provide filter capabilities for all endpoints that return a list of elements, as well should be able to support pagination.
 - The API filtering should allow using parenthesis for defining operations precedence and use any combination of the available fields. The supported operations should at least include or, and, eq (equals), ne (not equals), gt (greater than), lt (lower than). Example -> (date eq '2016-05-01') AND ((distance gt 20) OR (distance lt 10)).
 - Write unit tests.
# books-manager
## Overview
Write a REST API to manage a list of books (no frontend needed).  Use this data set: https://github.com/zygmuntz/goodbooks-10k/blob/master/books.csv . Only data about authors and titles is relevant.
* The books should be stored in SQL server and indexed in Elasticsearch.
* You should be able to add, update or delete books.
* You should be able to bulk add books from CSV data in the same format as the books.csv file.
* You should be able to add a review to any book. A review contains the email address of the reviewer and the review text. You can't update or delete reviews. When a beek is deleted so are the reviews.
* You should be able to search book authors and titles using Elasticsearch. Titles are more important than authors, so searching for "Tom" should return "The Adventures of Tom Sawyer" before books by Tom Clancy. When returning book info any reviews should be included too.
* Use EF Core as ORM and NEST as Elasticsearch client.
* Write some tests.

Allows to work with books: CRUD operations, add review, search for title and author.

# How to run
## Use docker-compose...
Go to the repository folder and make
```
sudo docker-compose build
sudo docker-compose up
```

Wait for SqlServer and Elasticsearch up (usually it's 30-40 seconds on my machine) and then run initial scripts
```
src/ef-update-database.sh
src/es-create-index.sh
```

## ... or run docker containers manually
Run SqlServer:
```
cd docker/sqlserver
sudo docker build . --tag sqlserver
sudo docker run -e 'ACCEPT_EULA=Y' -e 'MSSQL_SA_PASSWORD=Qwerty123!' -p 1433:1433 sqlserver
```

Run Elasticsearch
```
cd docker/elasticsearch
sudo docker build . --tag elasticsearch
sudo docker run -p 9200:9200 -p 9300:9300 -e "discovery.type=single-node" elasticsearch
```

Wait for both of them up and run the initial scripts
```
src/ef-update-database.sh
src/es-create-index.sh
```

Run the project
```
sudo docker build . --tag book-manager
sudo docker run -p 80:80 --net=host book-manager
```

# API
Here should be Swagger info, but it wasn't added yet, so just manual info:
- Get all books (paging) - 
```
GET /Books?skip=20&take=10
``` 
- Get specific book - 
```
GET /Books/1
```
- Add new book - 
```
POST /Books

{ "title": "My book", "author": "I am" }
```
- Update book -
```
PUT /Books/1

{ "title": "New title", "author": "New author" }
```
- Delete book - 
```
DELETE /Books/1
```
- Add review -
```
POST /Books/1/AddReview

{ "email": "test@example.com", "text": "Review text" }
```
- Search for books - `GET /Books/Search/Query+to+search?skip=0&take=10`

# How to load data
To load the whole books.csv file:
```
curl -XPOST "localhost/Books/BulkAdd" -T books.csv 
```

# In the end
Some things that I wanted to do, but I've already spent too much time:
- Async DB and ES calls. Nice to have, but not mandatory, I think, espesially for the test project
- Pretty simple analyzer and boost values for ES. Not very good, need better tuning, but anyway it solves the task 

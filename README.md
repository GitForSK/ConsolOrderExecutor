# ConsolOrderExecutor

## Instruction

1. Create database (SQL Server) and create tables using script in folder documents.
2. Create appsettings.json file with field ConnectionString tha holds database connection string.

## Description

This app allows to work on orders and their products. You can;

1. Create order
2. Move order to status 'W magazynie'
3. Move order to status 'W wysyłce'
4. Show products
5. Show orders
6. Modify product

## Notes

In order creation if product do not exist it will be created.

You can change order status to 'In warehouse' only when it's previous status is 'Nowe'. If order value is above 2500 and payment option is 'Gotówka przy odbiorze' it will move order to status 'Zwrócono do klienta'.

You can change order status to 'W wysyłce' only when it's previous status is 'W magazynie'. It's dealyed by 5 sec.

Show product have hardcoded pagination 5.
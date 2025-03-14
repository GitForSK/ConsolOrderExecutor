-- Tables

Create table Order_Status (
	status_id INT NOT NULL IDENTITY PRIMARY KEY,
	status_name VARCHAR(50) NOT NULL
);

Create table Payment_Option (
	payment_option_id INT NOT NULL IDENTITY PRIMARY KEY,
	option_name VARCHAR(50) NOT NULL
);

Create table App_Order (
	order_id INT NOT NULL IDENTITY PRIMARY KEY,
	isCompany BIT NOT NULL,
	delivery_address VARCHAR(250) NULL,
	status_id INT NOT NULL,
	payment_option_id INT NOT NULL,
	CONSTRAINT FK_Order_Order_status 
	FOREIGN KEY (status_id) REFERENCES Order_Status(status_id),
	CONSTRAINT FK_Order_Payment_Option 
	FOREIGN KEY (payment_option_id) REFERENCES Payment_Option(payment_option_id)
);

Create table App_Product (
	product_id INT NOT NULL IDENTITY PRIMARY KEY,
	product_ean VARCHAR(13) NOT NULL,
	product_name VARCHAR(150) NOT NULL
);

Create table Order_Product (
	order_id INT NOT NULL,
	product_id INT NOT NULL,
	price DECIMAL(30,2) NOT NULL,
	PRIMARY KEY (order_id, product_id),
	CONSTRAINT FK_Order_Id 
	FOREIGN KEY (order_id) REFERENCES App_Order(order_id),
	CONSTRAINT FK_Product_Id 
	FOREIGN KEY (product_id) REFERENCES App_Product(product_id),
);

-- seed

INSERT INTO Order_Status (status_name)
VALUES
('Nowe'),
('W magazynie'),
('W wysyłce'),
('Zwrócono do klienta'),
('Błąd'),
('Zamknięte');

INSERT INTO Payment_Option (option_name)
VALUES
('Karta'),
('Gotówka orzy odbiorze');
# asp-dotnet-core-web-api-cosmetics-shop

## Bảng Promotions
- Get all: /api/promotions
- Get: /api/promotions/{id?}
- Delete: /api/promotions/{id?}
- Post: /api/promotions
```
{
    "Name": "Tiki",
    "description": "Giảm giá tất cả sản phẩm",
    "discountRate": 60, ==>(Range: 1 -> 100)
    "startDate": "2022-03-20",
    "endDate": "2022-03-10" 
}
```
- Update: /api/promotions/{id?}
```
{
    "name": "Shopee",
    "description": "xuanphuc",
    "discountRate": 80, ==>(Range: 1 -> 100)
    "startDate": "2022-03-05",
    "endDate": "2022-03-10" 
}
```

## Bảng Categories
- Get all: /api/categories
- Get: /api/categories/{id?}
- Delete: /api/categories/{id?}
- Post: /api/categories
```
{
    "name": "Lazada 1",
    "image": "https://www.xuanphuc.space",
    "promotionId": 3 ==>(ID phải tồn tại)
}
```
- Update: /api/categories/{id?}
```
{
    "name": "Lazada 2",
    "image": "https://www.xuanphuc.space",
    "promotionId": 2 ==>(ID phải tồn tại)
}
```

## Bảng Product Options
- Get all: /api/productoptions
- Get: /api/productoptions/{id?}
- Delete: /api/productoptions/{id?}
- Post: /api/productoptions
```
{
    "name": "Color 1",
    "options": [
        {
            "name": "Tím",
            "value": "#000000"
        },
        {
            "name": "Hồng",
            "value": "#ffffff"
        }
    ]
}
```
- Update: /api/productoptions/{id?}
```
{
    "optionTypeId": 2, ==>(Không bắt buộc)
    "name": "Color",
    "options": [
        {
            "productOptionId": 5, ==>(ID option cũ)
            "name": "Tím",
            "value": "#000000",
            "optionTypeId": 2
        },
        {
            "productOptionId": 6, ==>(ID option cũ)
            "name": "Hồng",
            "value": "#ffffff",
            "optionTypeId": 2
        },
        {
            "name": "Tím",   ==>(Option mới không cần ID)
            "value": "#ffffff",
            "optionTypeId": 2
        }
    ]
}
```

## Bảng Products
- Get all: /api/products
- Get: /api/products/{id?}
- Delete: /api/products/{id?}
    - Chỉ được xóa sản phẩm khi chưa có đơn đặt hàng nào 😂
- Post: /api/products
```
{
	"name": "Tên sản phẩm",
	"description": "Mô tả sản phẩm",
	"image": "http://www.xuanphuc.space",
	"categoriesId": [2, 3],  ==>(ID phải tồn tại)
	"items": [
		{
            "sku": "SP001",
            "qtyInStock": 30,
            "image": "https://www.xuanphuc.space",
            "price": 200.00,
            "costPrice": 150.00,
            "optionsId": [2, 3, 4]  ==>(ID phải tồn tại)
        },
        {
            "sku": "SP002",
            "qtyInStock": 20,
            "image": "https://www.xuanphuc.space",
            "price": 300.00,
            "costPrice": 250.00,
            "optionsId": [8, 9]  ==>(ID phải tồn tại)
        }
	]
}
```
- Update: /api/products/{id?}
    - Chỉ được xóa loại sản phẩm (item) khi chưa có đặt đơn hàng nào
    - Chỉ được sửa cấu hình sản phẩm (options) khi chưa có đơn đặt hàng nào
```
{
    "productId": 1,  ==>(Không bắt buộc)
    "name": "Tên sản phẩm",
    "description": "Mô tả sản phẩm",
    "image": "http://www.xuanphuc.space",
    "categoriesId": [3],  ==>(ID phải tồn tại)
    "items": [
        {
            "productItemId": 4,  ==>(ID Item cũ)
            "sku": "SP001",
            "qtyInStock": 30,
            "image": "https://www.xuanphuc.space",
            "price": 200.00,
            "costPrice": 150.00,
            "productId": 1,  ==>(Không bắt buộc)
            "optionsId": [3, 4]  ==>(ID phải tồn tại)
        },
        {
            "sku": "SP002",  ==>(Item mới không cần ID)
            "qtyInStock": 20,
            "image": "https://www.xuanphuc.space",
            "price": 300.00,
            "costPrice": 250.00,
            "productId": 1,  ==>(Không bắt buộc)
            "optionsId": [5, 6]  ==>(ID phải tồn tại)
        }
    ]
}
```

## Shopping Cart
- Get all: /api/shoppingcarts
- Get: /api/shoppingcarts/{id?}
- Delete: /api/shoppingcarts/{id?}
- Post: /api/shoppingcarts
```
{
    "userId": "KH001",  ==>(Bắt buộc: ID User)
    "items": [
        {
            "productItemId": 8,  ==>(Bắt buộc: ID loại sản phẩm)
            "qty": 1
        }
    ]
}
```
- Update: /api/shoppingcarts/{id?}
```
{
    "cartId": 4,
    "userId": "KH001",
    "items": [
        {
            "cartItemId": 1,  ==>(ID sản phẩm cũ)
            "qty": 1,
            "cartId": 4,  ==>(Không bắt buộc: ID giỏ hàng)
            "productItemId": 8
        },
        {
            "qty": 1,  ==>(Thêm sản phẩm mới: không cần ID)
            "cartId": 4,  ==>(Không bắt buộc: ID giỏ hàng)
            "productItemId": 9
        }
    ]
}
```

## Shipping method
- Get all: /api/shippingmethods
- Get: /api/shippingmethods/{id?}
- Delete: /api/shippingmethods/{id?}
- Post: /api/shippingmethods
```
{
    "name": "Ninja Van",
    "price": 29000.00
}
```
- Update: /api/shippingmethods/{id?}
```
{
    "shippingMethodId": 5,  ==>(Không bắt buộc)
    "name": "Ninja Van",
    "price": 29000.00
}
```

## Order Status
- Get all: /api/orderstatuses
- Get: /api/orderstatuses/{id?}
- Delete: /api/orderstatuses/{id?}
- Post: /api/orderstatuses
```
{
    "status": "Đang chuẩn bị"
}
```
- Update: /api/shippingmethods/{id?}
```
{
    "oderStatusId": 1,  ==>(Không bắt buộc)
    "status": "Đang chuẩn bị"
}
```


## Payment Type
- Get all: /api/paymenttypes
- Get: /api/paymenttypes/{id?}
- Delete: /api/paymenttypes/{id?}
- Post: /api/paymenttypes
```
{
    "value": "xuanphuc"
}
```
- Update: /api/paymenttypes/{id?}
```
{
    "paymentTypeId": 3,
    "value": "xuanphuc"
}
```

## Payment Method
- Get all: /api/paymentmethods
- Get: /api/paymentmethods/{id?}
- Delete: /api/paymentmethods/{id?}
- Post: /api/paymentmethods
```
{
    "userId": "KH001",
    "paymentTypeId": 1,
    "provider": "Visa",
    "accountNumber": "23412341234123",
    "expiryDate": "2026-12-31",
    "isDefault": true
}
```
- Update: /api/paymentmethods/{id?}
```
{
    "paymentMethodId": 1,
    "provider": "Mastercard",
    "accountNumber": "23412341234123",
    "expiryDate": "2026-12-31T00:00:00",
    "isDefault": true,
    "paymentTypeId": 1,
    "userId": "KH001"
}
```

## Address
- Get all: /api/addresses
- Get: /api/addresses/{userid?}
- Delete: /api/addresses/{id?}
- Post: /api/addresses
```
{
    "userId": "KH001",
    "fullName": "Tran Xuan Phuc",
    "city": "Hanoi",
    "district": "Cau Giay",
    "ward": "Quan Hoa",
    "addressLine": "27, 79 Duong Quang Ham",
    "phoneNumber": "0923424523",
    "isDefault": true
}
```
- Update: /api/addresses/{id?}
```
{
    "addressId": 1,
    "fullName": "Tran Xuan Phuc",
    "city": "Hanoi",
    "district": "Cau Giay",
    "ward": "Quan Hoa",
    "addressLine": "79 Duong Quang Ham",
    "phoneNumber": "0923424523",
    "isDefault": true,
    "userId": "KH001"
}
```
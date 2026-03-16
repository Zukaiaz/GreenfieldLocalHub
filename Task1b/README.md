# Table AspNetUsers {

# &#x20; Id string \[pk]

# &#x20; UserName string

# &#x20; NormalizedUserName string

# &#x20; Email string

# &#x20; NormalizedEmail string

# &#x20; EmailConfirmed bool

# &#x20; PasswordHash string

# &#x20; PhoneNumber string

# }

# 

# Table AspNetRoles {

# &#x20; Id string \[pk]

# &#x20; Name string

# &#x20; NormalizedName string

# }

# 

# Table AspNetUserRoles {

# &#x20; UserId string

# &#x20; RoleId string

# }

# 

# Table AspNetUserClaims {

# &#x20; Id int \[pk]

# &#x20; UserId string

# &#x20; ClaimType string

# &#x20; ClaimValue string

# }

# 

# Table AspNetRoleClaims {

# &#x20; Id int \[pk]

# &#x20; RoleId string

# &#x20; ClaimType string

# &#x20; ClaimValue string

# }

# 

# Table AspNetUserLogins {

# &#x20; LoginProvider string

# &#x20; ProviderKey string

# &#x20; ProviderDisplayName string

# &#x20; UserId string

# }

# 

# Table AspNetUserTokens {

# &#x20; UserId string

# &#x20; LoginProvider string

# &#x20; Name string

# &#x20; Value string

# }

# 

# Table Producers {

# &#x20; Id int \[pk]

# &#x20; UserId string

# &#x20; FarmName varchar

# &#x20; Description text

# &#x20; Location varchar

# &#x20; ProductionMethod varchar

# &#x20; CreatedAt datetime

# }

# 

# Table Categories {

# &#x20; Id int \[pk]

# &#x20; Name varchar

# &#x20; Description text

# }

# 

# 

# Table Products {

# &#x20; Id int \[pk]

# &#x20; ProducerId int

# &#x20; CategoryId int

# &#x20; Name varchar

# &#x20; Description text

# &#x20; Price decimal

# &#x20; StockQuantity int

# &#x20; TraceabilityInfo text

# &#x20; IsAvailable bool

# &#x20; CreatedAt datetime

# }

# 

# Table Baskets {

# &#x20; Id int \[pk]

# &#x20; UserId string

# &#x20; CreatedAt datetime

# }

# 

# Table BasketItems {

# &#x20; Id int \[pk]

# &#x20; BasketId int

# &#x20; ProductId int

# &#x20; Quantity int

# }

# 

# Table Favourites {

# &#x20; Id int \[pk]

# &#x20; UserId string

# &#x20; ProductId int

# &#x20; CreatedAt datetime

# }

# 

# Table Orders {

# &#x20; Id int \[pk]

# &#x20; UserId string

# &#x20; OrderDate datetime

# &#x20; OrderStatus varchar

# &#x20; TotalAmount decimal

# }

# 

# Table OrderItems {

# &#x20; Id int \[pk]

# &#x20; OrderId int

# &#x20; ProductId int

# &#x20; Quantity int

# &#x20; UnitPrice decimal

# }

# 

# Table Deliveries {

# &#x20; Id int \[pk]

# &#x20; OrderId int

# &#x20; DeliveryType varchar

# &#x20; DeliveryAddress varchar

# &#x20; ScheduledDate datetime

# &#x20; Status varchar

# }

# 

# Table OrderTracking {

# &#x20; Id int \[pk]

# &#x20; OrderId int

# &#x20; Status varchar

# &#x20; UpdatedAt datetime

# &#x20; Notes text

# }

# 

# Table LoyaltyAccounts {

# &#x20; Id int \[pk]

# &#x20; UserId string

# &#x20; Points int

# &#x20; Tier varchar

# }

# 

# Table LoyaltyTransactions {

# &#x20; Id int \[pk]

# &#x20; LoyaltyAccountId int

# &#x20; PointsChange int

# &#x20; Reason varchar

# &#x20; CreatedAt datetime

# }

# 

# Table Offers {

# &#x20; Id int \[pk]

# &#x20; Title varchar

# &#x20; Description text

# &#x20; DiscountPercent decimal

# &#x20; StartDate datetime

# &#x20; EndDate datetime

# }

# 

# Table ProductOffers {

# &#x20; Id int \[pk]

# &#x20; ProductId int

# &#x20; OfferId int

# }

# 

# 

# Table SeasonalBoxes {

# &#x20; Id int \[pk]

# &#x20; ProducerId int

# &#x20; Name varchar

# &#x20; Description text

# &#x20; Season varchar

# &#x20; Price decimal

# &#x20; AvailableFrom datetime

# &#x20; AvailableUntil datetime

# }

# 

# Table SeasonalBoxItems {

# &#x20; Id int \[pk]

# &#x20; SeasonalBoxId int

# &#x20; ProductId int

# &#x20; Quantity int

# }

# 

# 

# // relationships

# 

# Ref: AspNetUsers.Id < AspNetUserRoles.UserId

# Ref: AspNetRoles.Id < AspNetUserRoles.RoleId

# 

# Ref: AspNetUsers.Id < AspNetUserClaims.UserId

# Ref: AspNetRoles.Id < AspNetRoleClaims.RoleId

# 

# Ref: AspNetUsers.Id < AspNetUserLogins.UserId

# Ref: AspNetUsers.Id < AspNetUserTokens.UserId

# 

# Ref: AspNetUsers.Id < Producers.UserId

# 

# Ref: Producers.Id < Products.ProducerId

# Ref: Categories.Id < Products.CategoryId

# 

# Ref: AspNetUsers.Id < Baskets.UserId

# Ref: Baskets.Id < BasketItems.BasketId

# Ref: Products.Id < BasketItems.ProductId

# 

# Ref: AspNetUsers.Id < Favourites.UserId

# Ref: Products.Id < Favourites.ProductId

# 

# Ref: AspNetUsers.Id < Orders.UserId

# Ref: Orders.Id < OrderItems.OrderId

# Ref: Products.Id < OrderItems.ProductId

# 

# Ref: Orders.Id < Deliveries.OrderId

# Ref: Orders.Id < OrderTracking.OrderId

# 

# Ref: AspNetUsers.Id < LoyaltyAccounts.UserId

# Ref: LoyaltyAccounts.Id < LoyaltyTransactions.LoyaltyAccountId

# 

# Ref: Products.Id < ProductOffers.ProductId

# Ref: Offers.Id < ProductOffers.OfferId

# 

# Ref: Producers.Id < SeasonalBoxes.ProducerId

# Ref: SeasonalBoxes.Id < SeasonalBoxItems.SeasonalBoxId

# Ref: Products.Id < SeasonalBoxItems.ProductId


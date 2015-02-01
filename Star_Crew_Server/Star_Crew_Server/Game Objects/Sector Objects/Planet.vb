Public Class Planet 'An object which holds economic information for the Sector
    Private Shared ReadOnly PlanetNames() As String = {
        "Deuses", "Dominus", "Caelum", "Vivificantem", "Moriendum", "Sceleri", "Divitiae"}
    Public name As String = PlanetNames(Int(Rnd() * PlanetNames.Length)) 'The Name of the Planet
    Public Money As New Game_Library.Game_Objects.StatInt(0, (Int(150000 * Rnd()) + 50000), Integer.MaxValue, True) 'The amount of money this Planet has
    Private dailyMoneyFluxuation As Integer = (Int(Rnd() * 10001) - 5000) 'An Integer value representing how the Planet's monet fluxuates weekly
    Public Enum ProductTypes 'The different types of products that can be purchased or sold
        Missiles
        Bullets
        Fuel
        max
    End Enum
    Private products() As Game_Library.Game_Objects.StatInt = {
        New Game_Library.Game_Objects.StatInt(0, Int(Rnd() * 201), 200, True),
        New Game_Library.Game_Objects.StatInt(0, Int(Rnd() * 40001), 40000, True),
        New Game_Library.Game_Objects.StatInt(0, Int(Rnd() * 500001), 500000, True)} 'An array of StatInt objects representing the current stock of the different products
    Private MaxTotalProducts As Integer = products(0).Maximum + products(1).Maximum + products(2).Maximum 'An Integer value indicating the maximum total number of products a Planet can store
    Private dailyProductFluxuation() As Integer = {
        Int(Rnd() * 11) - 5,
        Int(Rnd() * 2001) - 1000,
        Int(Rnd() * 25000) - 12500} 'An array of Integers representing how the stock of different products fluxuates weekly
    Private productPrices() As Integer = {
        MaxTotalProducts / products(0).Current + 1,
        MaxTotalProducts / products(1).Current + 1,
        MaxTotalProducts / products(2).Current + 1} 'An array of Integers representing the prices of different products
    Public Enum ResourceTypes 'The different types of resources that can be purchased or sold
        Metal
        Alloys
        Food
        Water
        Electronics
        Medical
        max
    End Enum
    Private resources() As Game_Library.Game_Objects.StatInt = {
        New Game_Library.Game_Objects.StatInt(0, Int(Rnd() * 11501), 11500, True),
        New Game_Library.Game_Objects.StatInt(0, Int(Rnd() * 14301), 14300, True),
        New Game_Library.Game_Objects.StatInt(0, Int(Rnd() * 20001), 20000, True),
        New Game_Library.Game_Objects.StatInt(0, Int(Rnd() * 20001), 20000, True),
        New Game_Library.Game_Objects.StatInt(0, Int(Rnd() * 15001), 15000, True),
        New Game_Library.Game_Objects.StatInt(0, Int(Rnd() * 8001), 8000, True)} 'An array of StatInt objects representing the current stock of the different resources
    Private MaxTotalResources As Integer 'An Integer value indicating the maximum total number of resources a Planet can store
    Private dailyResourceFluxuation() As Integer = {
        Int(Rnd() * 576) - 287,
        Int(Rnd() * 716) - 357,
        Int(Rnd() * 1001) - 500,
        Int(Rnd() * 1001) - 500,
        Int(Rnd() * 751) - 375,
        Int(Rnd() * 41) - 20} 'An array of Integers representing how the stock of different resources fluxuates weekly
    Private resourcePrices() As Integer = {
        MaxTotalResources / resources(0).Current + 1,
        MaxTotalResources / resources(1).Current + 1,
        MaxTotalResources / resources(2).Current + 1,
        MaxTotalResources / resources(3).Current + 1,
        MaxTotalResources / resources(4).Current + 1,
        MaxTotalResources / resources(5).Current + 1} 'An array of Integers representing the prices of different resources

    Public Sub Update() 'Update the Planet for this day
        Try
            Money.Current += dailyMoneyFluxuation 'Change the current funds of the Planet
            dailyMoneyFluxuation += Int(Rnd() * 2001) - 1000 'Change the weekly fluxuation of money
            For i As Integer = 0 To ProductTypes.max - 1 'Loop through all products
                products(i).Current += dailyProductFluxuation(i) 'Change the current stock of this product
                dailyProductFluxuation(i) += Int(Rnd() * 201) - 100 'Change the weekly fluxuation of this product
                productPrices(i) = If((products(i).Current <> 0),
                                      (MaxTotalProducts / products(i).Current),
                                      (MaxTotalProducts)) 'Recalculate the price of this product
            Next
            For i As Integer = 0 To ResourceTypes.max - 1 'Loop through all resources
                resources(i).Current += dailyResourceFluxuation(i) 'Change the current stock of this resource
                dailyResourceFluxuation(i) += Int(Rnd() * 1501) - 750 'Change the weekly fluxuation of this resource
                resourcePrices(i) = If((resources(i).Current <> 0),
                                       (MaxTotalResources / resources(i).Current),
                                       (MaxTotalResources)) 'Recalculate the price of this resource
            Next
        Catch ex As System.OverflowException
            dailyMoneyFluxuation = 0 'Set the money fluxuation to 0
            For i As Integer = 0 To ProductTypes.max - 1 'Loop through all products
                dailyProductFluxuation(i) = 0
            Next
            For i As Integer = 0 To ResourceTypes.max - 1 'Loop through all resources
                dailyResourceFluxuation(i) = 0
            Next
        End Try
    End Sub

    Public Function Buy_Product(ByVal product As ProductTypes, ByVal quantity As Integer, ByVal confirmed As Boolean) As Integer 'Returns an Integer representing the cost of a quantity of a product and optionally performs the purchase
        If quantity > products(product).Current Then 'This is an improper request
            Server.Write_To_Log(
                Environment.NewLine + "ERROR : A request was made for more of " + product.ToString() + " than was in stock." +
                Environment.NewLine + Environment.StackTrace)
            End
        End If
        Dim cost As Integer = productPrices(product) * quantity 'Calculate the cost of this purchase
        If confirmed Then 'Make the purchase
            Money.Current += cost 'Add the cost to the Planet's funds
            products(product).Current -= quantity 'Remove the quantity from the stock
        End If
        Return cost 'Return the cost
    End Function
    Public Function Sell_Product(ByVal product As ProductTypes, ByVal quantity As Integer, ByVal confirmed As Boolean) As Integer 'Return an Integer representing the payment for a quantity of a product and optionally performs the sale
        Dim payment As Integer = productPrices(product) * quantity 'Calculate how much will be paid for this quantity
        If payment > Money.Current Then payment = Money.Current 'If the payment is more than the Planet has bring it down to the maximum payable amount
        If confirmed Then 'Make the sale
            Money.Current -= payment 'Subtract the payment from the Planet's funds
            products(product).Current += quantity 'Add the product to the stock
        End If
        Return payment 'Return the payment
    End Function

    Public Function Buy_Resource(ByVal resource As ResourceTypes, ByVal quantity As Integer, ByVal confirmed As Boolean) As Integer 'Returns an Integer representing the cost of a quantity of a resource and optionally performs the purchase
        If quantity > resources(resource).Current Then 'This is an improper request
            Server.Write_To_Log(
                Environment.NewLine + "ERROR : A request was made for more of " + resource.ToString() + " than was in stock." +
                Environment.NewLine + Environment.StackTrace)
            End
        End If
        Dim cost As Integer = resourcePrices(resource) * quantity 'Calculate the cost of this purchase
        If confirmed Then 'Make the purchase
            Money.Current += cost 'Add the cost to the Planet's funds
            resources(resource).Current -= quantity 'Remove the quantity from the stock
        End If
        Return cost 'Return the cost
    End Function
    Public Function Sell_Resource(ByVal resource As ResourceTypes, ByVal quantity As Integer, ByVal confirmed As Boolean) As Integer 'Return an Integer representing the payment for a quantity of a resource and optionally performs the sale
        Dim payment As Integer = resourcePrices(resource) * quantity 'Calculate how much will be paid for this quantity
        If payment > Money.Current Then payment = Money.Current 'If the payment is more than the Planet has bring it down to the maximum payable amount
        If confirmed Then 'Make the sale
            Money.Current -= payment 'Subtract the payment from the Planet's funds
            resources(resource).Current += quantity 'Add the resource to the stock
        End If
        Return payment 'Return the payment
    End Function

End Class

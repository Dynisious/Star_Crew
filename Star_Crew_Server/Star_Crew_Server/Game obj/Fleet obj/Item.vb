Public MustInherit Class Item
    Public Index As Integer 'An Integer value representing the Item's index in the Fleet's/Planet's inventory
    Public StackSize As Integer 'An Integer value representing how many of this Item are stored in this slot

    Public MustOverride Sub Use(ByRef data As Object) 'Uses the Item

End Class

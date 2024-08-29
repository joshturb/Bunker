using System;


public enum ItemType
{
    None = -1,
    BuildingPlan = 0,
}

public enum ItemCategory
{
    None = 0,
    Building = 1,
}


[Flags]
public enum ItemTier: byte
{
	Common = 0,
	Rare = 1,
	ExtraRare = 2
}

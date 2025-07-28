# Data Drafts




## Draft: Reverse Course: Enums: ClassType
## Agent
So we're finding that our enums are causing more issues than they're helping. We want to remove the ClassType enum and instead use a string. This will allow us to have more flexibility in the future. This means any helper can be removed and any properties currently going to an enum should go to a string of max length 63. 

## Draft: Reverse Course: Enums: InventoryType
## Agent
So we're finding that our enums are causing more issues than they're helping. We want to remove the InventoryType enum and instead use a string. This will allow us to have more flexibility in the future. This means any helper can be removed and any properties currently going to an enum should go to a string of max length 127. 

## Draft: Reverse Course: Enums: SlotType
## Agent
So we're finding that our enums are causing more issues than they're helping. We want to remove the SlotType enum and instead use a string. This will allow us to have more flexibility in the future. This means any helper can be removed and any properties currently going to an enum should go to a string of max length 63. 

## Draft: Reverse Course: Enums: SubclassType
## Agent
So we're finding that our enums are causing more issues than they're helping. We want to remove the SubclassType enum and instead use a string. This will allow us to have more flexibility in the future. This means any helper can be removed and any properties currently going to an enum should go to a string of max length 63. This currently maps the classtype with the subclass type. but we just need the string for the subclass now. 
# Warbound Addon Drafts

## Draft: Vendor Scraping Logging
### Developer
Use GetMerchantNumItems and GetMerchantItemID and GetMerchantItemInfo to loop through a vendor's wares and create an object that can encapsulate all of the missing data about an item. Just get it in an object. We'll figure out how to serialize it and get it out of SavedVariables later.

## Draft: Create A Vendor Provider Table
### Developer
Using the data from the last draft, we need to create an id-centric table that will store all of the details found via LUA. We probably want to store when it was added or updated last. 

## Draft: Hook it All Up
### Developer
Migrate the table, serialize the object we see (likely 'p1,p2,p3;' format), save it into saved variables, and then let's pull it out via extending the 'AutoPublisher' or having it call something more specific - and put it in the database.

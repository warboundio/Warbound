# Data Drafts

## Draft: Models Folder
### Agent
Right now we have a lot of 'Models' in the endpoints folder in the Data project. Can we move these to a Models folder inside the Data project? This will help keep things organized and make it easier to find the models we need. Specifically we want any enums to go to the enum folder and any of the models that are a part of the blizzartapicontext to go to the Models folder. Anything in the Endpoints folder is in scope. Don't move other files. Ensure all tests pass afterwards.

## Draft: Enums
### Agent
I love having UNKNOWN = -1. It's such a great pattern. However, with our Encoding we're looking to do -1 isn't supported. it's all 0-X for base90 encoding. So what we really need is to have a new pattern. Which I'm not wild about but it'll work. Check out ClassType enum. It has UNKNOWN as -1 but it also has a value as UNKNOWN_ENCODING. And this is above 0. So what value does it need to be? Well if the max number is below 70  already let's make it 89 as that's the max value for a base90 encoding. If there are no more optinos available and we have to (or have already moved into 90+ values) then we're looking at 90 ^ 2 - 1 which appears to be 8099. So we can use that as a value. So the pattern is UNKNOWN = -1, UNKNOWN_ENCODING = 89 or 8099 depending on the max value of the enum. So please look through all enums in this data project and update them to this pattern. If the enum is already using this pattern then leave it alone. If the enum is not using this pattern then update it to use this pattern. Ensure all tests pass afterwards.
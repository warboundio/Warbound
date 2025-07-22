WarboundEventManager = {}
WarboundEventManager.handlers = {}

local frame = CreateFrame("Frame")
frame:SetScript("OnEvent", function(self, event, ...)
  WarboundEventManager:Dispatch(event, ...)
end)

function WarboundEventManager:On(eventName, handlerFn)
  if not self.handlers[eventName] then
    self.handlers[eventName] = {}
    frame:RegisterEvent(eventName)
  end
  table.insert(self.handlers[eventName], handlerFn)
end

function WarboundEventManager:Dispatch(eventName, ...)
  for _, fn in ipairs(self.handlers[eventName] or {}) do
    fn(...)
  end
end

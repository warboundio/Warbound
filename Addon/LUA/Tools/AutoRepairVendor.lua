WarboundEventManager:On("MERCHANT_SHOW", function()
  if CanMerchantRepair() then
    WarboundLogger:Debug('Repairing all items')
    RepairAllItems()
  end

  if MerchantFrameSellJunkButton and MerchantFrameSellJunkButton:IsEnabled() then
    MerchantFrameSellJunkButton:Click()
    WarboundLogger:Debug('Vendoring junk')
  end
end)
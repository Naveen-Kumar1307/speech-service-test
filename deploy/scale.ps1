Set-AzContext -SubscriptionId "SUBSCRIPTION_ID"
$rule1=New-AzAutoscaleScaleRuleObject `
    -MetricTriggerMetricName "Percentage CPU" `
    -MetricTriggerMetricResourceUri "/subscriptions/SUBSCRIPTION_ID/resourceGroups/RESOURCE_GROUP_NAME/providers/Microsoft.Compute/virtualMachineScaleSets/VMSS_NAME"  `
    -MetricTriggerTimeGrain ([System.TimeSpan]::New(0,1,0)) `
    -MetricTriggerStatistic "Average" `
    -MetricTriggerTimeWindow ([System.TimeSpan]::New(0,5,0)) `
    -MetricTriggerTimeAggregation "Average" `
    -MetricTriggerOperator "GreaterThan" `
    -MetricTriggerThreshold 70 `
    -MetricTriggerDividePerInstance $false `
    -ScaleActionDirection "Increase" `
    -ScaleActionType "ChangeCount" `
    -ScaleActionValue 1 `
    -ScaleActionCooldown ([System.TimeSpan]::New(0,5,0))
$rule2=New-AzAutoscaleScaleRuleObject `
    -MetricTriggerMetricName "Percentage CPU" `
    -MetricTriggerMetricResourceUri "/subscriptions/SUBSCRIPTION_ID/resourceGroups/RESOURCE_GROUP_NAME/providers/Microsoft.Compute/virtualMachineScaleSets/VMSS_NAME"  `
    -MetricTriggerTimeGrain ([System.TimeSpan]::New(0,1,0)) `
    -MetricTriggerStatistic "Average" `
    -MetricTriggerTimeWindow ([System.TimeSpan]::New(0,5,0)) `
    -MetricTriggerTimeAggregation "Average" `
    -MetricTriggerOperator "LessThan" `
    -MetricTriggerThreshold 30 `
    -MetricTriggerDividePerInstance $false `
    -ScaleActionDirection "Decrease" `
    -ScaleActionType "ChangeCount" `
    -ScaleActionValue 1 `
    -ScaleActionCooldown ([System.TimeSpan]::New(0,5,0))
$defaultProfile=New-AzAutoscaleProfileObject -Name "default" -CapacityDefault "SCALE_OUT_CAPACITY_MINIMUM" -CapacityMaximum "CAPACITY_MAXIMUM" -CapacityMinimum "SCALE_OUT_CAPACITY_MINIMUM" -Rule $rule1, $rule2
Update-AzAutoscaleSetting  -Name "AZURE_AUTOSCALING_NAME" -ResourceGroupName RESOURCE_GROUP_NAME -Profile $defaultProfile -TargetResourceUri "/subscriptions/SUBSCRIPTION_ID/resourceGroups/RESOURCE_GROUP_NAME/providers/Microsoft.Compute/virtualMachineScaleSets/VMSS_NAME"
$getVMInstances = Get-AzVmssVM -InstanceView -ResourceGroupName "RESOURCE_GROUP_NAME" -VMScaleSetName "VMSS_NAME"
$getVM= Get-AzVmssVM -ResourceGroupName "RESOURCE_GROUP_NAME" -VMScaleSetName "VMSS_NAME"
while( ($getVMInstances.InstanceView.VmHealth.Status.Code -eq "HealthState/unhealthy") -or (($getVM.ProvisioningState | Select -Unique).Count -ne 1 ) ) { $getVMInstances = Get-AzVmssVM -InstanceView -ResourceGroupName "RESOURCE_GROUP_NAME" -VMScaleSetName "VMSS_NAME";  $getVM= Get-AzVmssVM -ResourceGroupName "RESOURCE_GROUP_NAME" -VMScaleSetName "VMSS_NAME"; Start-Sleep -Seconds 30; }
$defaultProfile=New-AzAutoscaleProfileObject -Name "default" -CapacityDefault "SCALE_IN_CAPACITY_MINIMUM" -CapacityMaximum "CAPACITY_MAXIMUM" -CapacityMinimum "SCALE_IN_CAPACITY_MINIMUM" -Rule $rule1, $rule2
Update-AzAutoscaleSetting  -Name "AZURE_AUTOSCALING_NAME" -ResourceGroupName RESOURCE_GROUP_NAME -Profile $defaultProfile -TargetResourceUri "/subscriptions/SUBSCRIPTION_ID/resourceGroups/RESOURCE_GROUP_NAME/providers/Microsoft.Compute/virtualMachineScaleSets/VMSS_NAME"
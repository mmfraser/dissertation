﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;


namespace WebService {
    public class WorkOrderSvc : IWorkOrderSvc {
        
        public BusinessLayer.WorkOrder CreateWorkOrder(String at, int deviceId, int applicationId, string commPackageJson) {
            new AuthSvc().AuthUser(at, -1, deviceId);

            BusinessLayer.WorkOrder wo = BusinessLayer.WorkOrder.CreateWorkOrder(deviceId, applicationId, commPackageJson);

           CloudQueues.NewWorkOrderQueueClient.Send(new BrokeredMessage(wo.WorkOrderId));

           return wo;
        }

        public WorkOrderTrimmed GetWorkOrder(String at, int deviceId, int workOrderId) {
            new AuthSvc().AuthUser(at, -1, deviceId);
            BusinessLayer.WorkOrder wo = BusinessLayer.WorkOrder.Populate(workOrderId);

            WorkOrderTrimmed wt = new WorkOrderTrimmed();
            wt.ApplicationId = wo.ApplicationId;
            wt.CommPackageJson = wo.CommPackageJson;
            wt.DeviceId = wo.DeviceId;
            wt.ReceiveTime = wo.ReceiveTime;
            wt.SlaveWorkerId = wo.SlaveWorkerId;
            wt.SlaveWorkerSubmit = wo.SlaveWorkerSubmit;
            wt.SlaveWorkOrderLastCommunication = wo.SlaveWorkOrderLastCommunication;
            wt.WorkOrderId = wo.WorkOrderId;
            wt.WorkOrderResultJson = wo.WorkOrderResultJson;
            wt.WorkOrderStatus = wo.WorkOrderStatus;
            wt.ComputeAppIntent = wo.WorkApplication.ApplicationWorkIntent;
            
            return wt;
        }

        public void CancelWorkOrder(String at, int workOrderId) {
            BusinessLayer.AuthenticationToken oAt = new AuthSvc().AuthUser(at);
            BusinessLayer.WorkOrder wo = BusinessLayer.WorkOrder.Populate(workOrderId);

            if (wo.DeviceId != oAt.DeviceId)
                throw new Exception("Cannot delete Work Order which you do not own");

            CloudQueues.UpdatedWorkOrderQueueClient.Send(new BrokeredMessage(new SharedClasses.WorkOrderUpdate(workOrderId, SharedClasses.WorkOrderUpdate.UpdateType.Cancel, oAt.DeviceId)));
            
        }

        public void AcknowledgeWorkOrder(String at, int workOrderId) {
            BusinessLayer.AuthenticationToken oAt = new AuthSvc().AuthUser(at);
            BusinessLayer.WorkOrder wo = BusinessLayer.WorkOrder.Populate(workOrderId);

            if (wo.SlaveWorkerId != oAt.DeviceId)
                throw new Exception("Cannot delete Work Order which you are not meant to be working on.");

            CloudQueues.UpdatedWorkOrderQueueClient.Send(new BrokeredMessage(new SharedClasses.WorkOrderUpdate(workOrderId, SharedClasses.WorkOrderUpdate.UpdateType.Acknowledge, oAt.DeviceId)));

        }

        public void SubmitWorkOrderResult(string at, int workOrderId, String resultJson) {
            BusinessLayer.AuthenticationToken oAt = new AuthSvc().AuthUser(at);
            BusinessLayer.WorkOrder wo = BusinessLayer.WorkOrder.Populate(workOrderId);

            if (wo.SlaveWorkerId != oAt.DeviceId)
                throw new Exception("Cannot modify Work Order which you are not meant to be working on.");

            CloudQueues.UpdatedWorkOrderQueueClient.Send(new BrokeredMessage(new SharedClasses.WorkOrderUpdate(workOrderId, SharedClasses.WorkOrderUpdate.UpdateType.SubmitResult, oAt.DeviceId, resultJson)));

        }

    }
}

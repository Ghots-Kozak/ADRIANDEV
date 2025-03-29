using Abixe_Models;

namespace ControlFloor.Models
{
    public class ResponseAbx
    {

        public string Token { get; set; }
        public string Message { get; set; }
        // public string IdRole { get; set; }
        public object JsonRsp { get; set; }

        public string Version { get; set; }
        public bool IsError { get; set; }

        public string EntryObject { get; set; }

    }

    public class ProductionOrderResponse
    {
        public string OdataMetadata { get; set; }
        public int AbsoluteEntry { get; set; }
        public int DocumentNumber { get; set; }
        public int Series { get; set; }
        public string ItemNo { get; set; }
        public string ProductionOrderStatus { get; set; }
        public string ProductionOrderType { get; set; }
        public decimal PlannedQuantity { get; set; }
        public decimal CompletedQuantity { get; set; }
        public decimal RejectedQuantity { get; set; }
        public DateTime PostingDate { get; set; }
        public DateTime DueDate { get; set; }
        //public int ProductionOrderOriginEntry { get; set; }
        //public int ProductionOrderOriginNumber { get; set; }
        public string ProductionOrderOrigin { get; set; }
        public int UserSignature { get; set; }
        public string Remarks { get; set; }
        public DateTime? ClosingDate { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string CustomerCode { get; set; }
        public string Warehouse { get; set; }
        public string InventoryUOM { get; set; }
        public string JournalRemarks { get; set; }
        public string TransactionNumber { get; set; }
        public DateTime CreationDate { get; set; }
        public string Printed { get; set; }
        public string DistributionRule { get; set; }
        public string Project { get; set; }
        public string DistributionRule2 { get; set; }
        public string DistributionRule3 { get; set; }
        public string DistributionRule4 { get; set; }
        public string DistributionRule5 { get; set; }
        public int UoMEntry { get; set; }
        public DateTime StartDate { get; set; }
        public string ProductDescription { get; set; }
        public int Priority { get; set; }
        public string RoutingDateCalculation { get; set; }
        public string UpdateAllocation { get; set; }
        public string SAPPassport { get; set; }
        public int? AttachmentEntry { get; set; }
        public string U_ABIXE_WMS_IdUser { get; set; }
        public List<ProductionOrderLine> ProductionOrderLines { get; set; }
        public List<object> ProductionOrdersSalesOrderLines { get; set; }
        public List<object> ProductionOrdersStages { get; set; }
        public List<object> ProductionOrdersDocumentReferences { get; set; }
    }

    public class ProductionOrderLine
    {
        public int DocumentAbsoluteEntry { get; set; }
        public int LineNumber { get; set; }
        public string ItemNo { get; set; }
        public decimal BaseQuantity { get; set; }
        public decimal PlannedQuantity { get; set; }
        public decimal IssuedQuantity { get; set; }
        public string ProductionOrderIssueType { get; set; }
        public string Warehouse { get; set; }
        public int VisualOrder { get; set; }
        public string DistributionRule { get; set; }
        public string LocationCode { get; set; }
        public string Project { get; set; }
        public string DistributionRule2 { get; set; }
        public string DistributionRule3 { get; set; }
        public string DistributionRule4 { get; set; }
        public string DistributionRule5 { get; set; }
        public int UoMEntry { get; set; }
        public int UoMCode { get; set; }
        public string WipAccount { get; set; }
        public string ItemType { get; set; }
        public string LineText { get; set; }
        public decimal AdditionalQuantity { get; set; }
        public string ResourceAllocation { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string StageID { get; set; }
        public decimal RequiredDays { get; set; }
        public string ItemName { get; set; }
        public List<string> SerialNumbers { get; set; }
        public List<string> BatchNumbers { get; set; }
    }

    public class DocumentsResponse
    {
        public string OdataMetadata { get; set; }
        public int DocEntry { get; set; }
        public int? DocNum { get; set; }

        //public int? ServiceCallID { get; set; }
        
        public List<DocumentLines> DocumentLines { get; set; }
    }


    public class DocumentLines
    {
        public int DocEntry { get; set; }
        public int LineNumbe { get; set; }
        public string ItemCode { get; set; }
    }

    public class Attachments2
    {
        public string odata_metadata { get; set; }
        public int AbsoluteEntry { get; set; }
        public List<AttachmentLine> Attachments2_Lines { get; set; }
    }

    public class AttachmentLine
    {
        public int AbsoluteEntry { get; set; }
        public int LineNum { get; set; }
        public string SourcePath { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public DateTime AttachmentDate { get; set; }
        public string Override { get; set; }
        public string FreeText { get; set; }
        public string CopyToTargetDoc { get; set; }
        public string CopyToProductionOrder { get; set; }
    }


    public class TrasferResponse
    {
        public string OdataMetadata { get; set; }
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public List<DocumentLines> StockTransferLines { get; set; }
    }

    public class ActivitiesResponse
    {
        public int? ActivityCode { get; set; }
        public int? DocEntry { get; set; }
    }


}

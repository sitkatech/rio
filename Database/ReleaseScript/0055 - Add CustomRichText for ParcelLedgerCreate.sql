insert into dbo.CustomRichTextType (CustomRichTextTypeID, CustomRichTextTypeName, CustomRichTextTypeDisplayName)
values (23, 'ParcelLedgerCreate', 'Create New Transaction')

insert into dbo.CustomRichText (CustomRichTextTypeID, CustomRichTextContent)
values (23, '<p>Use this form to perform a water deposit or withdrawal transaction for an individual parcel.</p><p>Example uses of the form:</p><ul><li><strong>Adjust an automatically generated water deposit. </strong>For example: Remotely sensed water usage assumed a different crop type and landowner has provided electrical use data to support a lower volume of usage in July.</li><li><strong>Credit a parcel with independently acquired water. </strong>For example: A landowner has purchased groundwater independently and the volume needs to be associated with their water budget.</li><li><strong>Correct a data entry error. </strong>For example: a user transcribed a digit when creating a previous transaction, and needs to fix the error; rather than deleting the previous transaction, a new transaction should be created to offset the error.</li></ul>')
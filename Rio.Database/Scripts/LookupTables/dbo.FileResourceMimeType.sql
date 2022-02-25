Merge into dbo.FileResourceMimeType As Target 
Using (Values
(1, 'application/pdf', 'PDF', 'PDF', '/Content/img/MimeTypeIcons/pdf_20x20.png', '/Content/img/MimeTypeIcons/pdf_48x48.png'),
 -- Word document (.docx)																	  							                                  
(2, 'application/vnd.openxmlformats-officedocument.wordprocessingml.document', 'Word (DOCX)', 'Word (DOCX)', '/Content/img/MimeTypeIcons/word_20x20.png', '/Content/img/MimeTypeIcons/word_48x48.png'),
-- Excel file (.xlsx) 																		  							                                 
(3, 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', 'Excel (XLSX)', 'Excel (XLSX)', '/Content/img/MimeTypeIcons/excel_20x20.png', '/Content/img/MimeTypeIcons/excel_48x48.png'),
(15, 'application/x-excel', 'x-Excel (XLSX)', 'x-Excel (XLSX)', '/Content/img/MimeTypeIcons/excel_20x20.png', '/Content/img/MimeTypeIcons/excel_48x48.png'),

-- X-png																					  							                                 
(4, 'image/x-png', 'X-PNG', 'X-PNG', null, null),
(5, 'image/png', 'PNG', 'PNG', null, null),
(6, 'image/tiff', 'TIFF', 'TIFF', null, null),
(7, 'image/bmp', 'BMP', 'BMP', null, null),
(8, 'image/gif', 'GIF', 'GIF', null, null),
(9, 'image/jpeg', 'JPEG', 'JPEG', null, null),
(10, 'image/pjpeg', 'PJPEG', 'PJPEG', null, null),
-- ppt
(11, 'application/vnd.openxmlformats-officedocument.presentationml.presentation', 'Powerpoint (PPTX)', 'Powerpoint (PPTX)', '/Content/img/MimeTypeIcons/powerpoint_20x20.png', '/Content/img/MimeTypeIcons/powerpoint_48x48.png'),
(12, 'application/vnd.ms-powerpoint', 'Powerpoint (PPT)', 'Powerpoint (PPT)', '/Content/img/MimeTypeIcons/powerpoint_20x20.png', '/Content/img/MimeTypeIcons/powerpoint_48x48.png'),
(13, 'application/vnd.ms-excel', 'Excel (XLS)', 'Excel (XLS)', '/Content/img/MimeTypeIcons/excel_20x20.png', '/Content/img/MimeTypeIcons/excel_48x48.png'),
(14, 'application/msword', 'Word (DOC)', 'Word (DOC)', '/Content/img/MimeTypeIcons/word_20x20.png', '/Content/img/MimeTypeIcons/word_48x48.png'),
--
(16, 'text/css', 'CSS', 'CSS', null, null),
-- ZIP
(17, 'application/x-zip-compressed', 'ZIP', 'ZIP', null, null)
)
As Source (FileResourceMimeTypeID, FileResourceMimeTypeContentTypeName, FileResourceMimeTypeName, FileResourceMimeTypeDisplayName, FileResourceMimeTypeIconSmallFilename, FileResourceMimeTypeIconNormalFilename)
On Target.FileResourceMimeTypeID = Source.FileResourceMimeTypeID
WHEN MATCHED THEN
UPDATE SET
	FileResourceMimeTypeContentTypeName = Source.FileResourceMimeTypeContentTypeName,
	FileResourceMimeTypeName = Source.FileResourceMimeTypeName,
	FileResourceMimeTypeDisplayName = Source.FileResourceMimeTypeDisplayName,
	FileResourceMimeTypeIconSmallFilename = Source.FileResourceMimeTypeIconSmallFilename,
	FileResourceMimeTypeIconNormalFilename = Source.FileResourceMimeTypeIconNormalFilename
WHEN NOT MATCHED BY TARGET THEN
	INSERT (FileResourceMimeTypeID, FileResourceMimeTypeContentTypeName, FileResourceMimeTypeName, FileResourceMimeTypeDisplayName, FileResourceMimeTypeIconSmallFilename, FileResourceMimeTypeIconNormalFilename)
	Values (FileResourceMimeTypeID, FileResourceMimeTypeContentTypeName, FileResourceMimeTypeName, FileResourceMimeTypeDisplayName, FileResourceMimeTypeIconSmallFilename, FileResourceMimeTypeIconNormalFilename)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;



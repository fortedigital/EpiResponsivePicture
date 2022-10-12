namespace Forte.EpiResponsivePicture.ResizedImage.Property.Compatibility.SqlProvider;

class ImageResizerFocalPointConversionSqlProvider : IImageResizerFocalPointConversionSqlProvider
{
    private const string DatabaseScheme = "[dbo]";
    private const string PropertyDefinitionTable = "[tblPropertyDefinition]";
    private const string PropertyDefinitionTypeTable = "[tblPropertyDefinitionType]";
    private const string WorkContentPropertyTable = "[tblPropertyDefinitionType]";
	
    public string Generate() =>
        @$";WITH [ForteEPiFocalPointPropertyDefinitionType] AS 
(
	SELECT [pkID] AS [ID]
	FROM {DatabaseScheme}.{PropertyDefinitionTypeTable}
	WHERE [TypeName] LIKE 'Forte.EpiResponsivePicture.ResizedImage.Property.PropertyFocalPoint'
),
[PropertyDefinition] AS 
(
	SELECT [pkID] AS [ID], [fkPropertyDefinitionTypeID] AS [PropertyDefinitionTypeID]
	FROM {DatabaseScheme}.{PropertyDefinitionTable}
	WHERE [Name] LIKE 'FocalPoint'
)
UPDATE [PropertyDefinition]
SET [PropertyDefinitionTypeID] = [ForteEPiFocalPointPropertyDefinitionType].[ID]
FROM [ForteEPiFocalPointPropertyDefinitionType]

GO

;WITH [FocalPointDefinitionType] AS
(
	SELECT [pkID] AS [ID], [fkPropertyDefinitionTypeID] AS [PropertyDefinitionTypeID]
	FROM {DatabaseScheme}.{PropertyDefinitionTable}
	WHERE [Name] LIKE 'FocalPoint'
)
UPDATE {DatabaseScheme}.{WorkContentPropertyTable}
SET [String] = [LongString]
FROM [FocalPointDefinitionType]
WHERE [fkPropertyDefinitionID] = [FocalPointDefinitionType].[ID]

GO";
}

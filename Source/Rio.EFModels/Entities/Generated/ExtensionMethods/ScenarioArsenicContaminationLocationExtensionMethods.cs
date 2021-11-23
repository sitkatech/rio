//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ScenarioArsenicContaminationLocation]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class ScenarioArsenicContaminationLocationExtensionMethods
    {
        public static ScenarioArsenicContaminationLocationDto AsDto(this ScenarioArsenicContaminationLocation scenarioArsenicContaminationLocation)
        {
            var scenarioArsenicContaminationLocationDto = new ScenarioArsenicContaminationLocationDto()
            {
                ScenarioArsenicContaminationLocationID = scenarioArsenicContaminationLocation.ScenarioArsenicContaminationLocationID,
                ScenarioArsenicContaminationLocationWellName = scenarioArsenicContaminationLocation.ScenarioArsenicContaminationLocationWellName
            };
            DoCustomMappings(scenarioArsenicContaminationLocation, scenarioArsenicContaminationLocationDto);
            return scenarioArsenicContaminationLocationDto;
        }

        static partial void DoCustomMappings(ScenarioArsenicContaminationLocation scenarioArsenicContaminationLocation, ScenarioArsenicContaminationLocationDto scenarioArsenicContaminationLocationDto);

        public static ScenarioArsenicContaminationLocationSimpleDto AsSimpleDto(this ScenarioArsenicContaminationLocation scenarioArsenicContaminationLocation)
        {
            var scenarioArsenicContaminationLocationSimpleDto = new ScenarioArsenicContaminationLocationSimpleDto()
            {
                ScenarioArsenicContaminationLocationID = scenarioArsenicContaminationLocation.ScenarioArsenicContaminationLocationID,
                ScenarioArsenicContaminationLocationWellName = scenarioArsenicContaminationLocation.ScenarioArsenicContaminationLocationWellName
            };
            DoCustomSimpleDtoMappings(scenarioArsenicContaminationLocation, scenarioArsenicContaminationLocationSimpleDto);
            return scenarioArsenicContaminationLocationSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(ScenarioArsenicContaminationLocation scenarioArsenicContaminationLocation, ScenarioArsenicContaminationLocationSimpleDto scenarioArsenicContaminationLocationSimpleDto);
    }
}
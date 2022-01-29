//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ScenarioRechargeBasin]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class ScenarioRechargeBasinExtensionMethods
    {
        public static ScenarioRechargeBasinDto AsDto(this ScenarioRechargeBasin scenarioRechargeBasin)
        {
            var scenarioRechargeBasinDto = new ScenarioRechargeBasinDto()
            {
                ScenarioRechargeBasinID = scenarioRechargeBasin.ScenarioRechargeBasinID,
                ScenarioRechargeBasinName = scenarioRechargeBasin.ScenarioRechargeBasinName,
                ScenarioRechargeBasinDisplayName = scenarioRechargeBasin.ScenarioRechargeBasinDisplayName
            };
            DoCustomMappings(scenarioRechargeBasin, scenarioRechargeBasinDto);
            return scenarioRechargeBasinDto;
        }

        static partial void DoCustomMappings(ScenarioRechargeBasin scenarioRechargeBasin, ScenarioRechargeBasinDto scenarioRechargeBasinDto);

        public static ScenarioRechargeBasinSimpleDto AsSimpleDto(this ScenarioRechargeBasin scenarioRechargeBasin)
        {
            var scenarioRechargeBasinSimpleDto = new ScenarioRechargeBasinSimpleDto()
            {
                ScenarioRechargeBasinID = scenarioRechargeBasin.ScenarioRechargeBasinID,
                ScenarioRechargeBasinName = scenarioRechargeBasin.ScenarioRechargeBasinName,
                ScenarioRechargeBasinDisplayName = scenarioRechargeBasin.ScenarioRechargeBasinDisplayName
            };
            DoCustomSimpleDtoMappings(scenarioRechargeBasin, scenarioRechargeBasinSimpleDto);
            return scenarioRechargeBasinSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(ScenarioRechargeBasin scenarioRechargeBasin, ScenarioRechargeBasinSimpleDto scenarioRechargeBasinSimpleDto);
    }
}
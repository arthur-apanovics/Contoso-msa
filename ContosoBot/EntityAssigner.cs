using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using ContosoBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ContosoBot
{
    [Serializable]
    public class EntityAssigner
    {
        public EntityProps AssignEntities(LuisResult luisResult)
        {
            EntityProps entityProps = new EntityProps();

            //Resolve LUIS entities
            foreach (var entity in luisResult.Entities)
            {
                //handle encycplopedia seperately due to multiple recommendations per encyclopedia entity
                if (entity.Type.Contains("builtin.encyclopedia") && string.IsNullOrEmpty(entityProps.Encyclopedia))
                {
                    entityProps.Encyclopedia = entity.Entity;
                    continue;
                }

                //handle multiple datetime resolution variants
                if (entity.Type.Contains("builtin.datetimeV2"))
                {
                    //dance around the fire. VS doesn't know what types the objects have, need to give it a hand...
                    var getTheListObj = entity.Resolution["values"] as List<object>;
                    var resolvedDict = getTheListObj[0] as Dictionary<string, object>;

                    //assign string values
                    var outTimex = resolvedDict["timex"].ToString();
                    var outType = resolvedDict["type"].ToString();
                    DateTime outStart = new DateTime();
                    DateTime outEnd = new DateTime();


                    //parse datetime values
                    if (outType == "date")
                    {
                        DateTime.TryParse(resolvedDict["value"].ToString(), out outStart);
                    }
                    else if (outType == "daterange" || outType == "datetimerange")
                    {
                        DateTime.TryParse(resolvedDict["start"].ToString(), out outStart);
                        DateTime.TryParse(resolvedDict["end"].ToString(), out outEnd);
                    }


                    //set property to parsed values
                    entityProps.DateRange = new DateRange
                    {
                        Timex = outTimex,
                        Type = outType,
                        Start = outStart,
                        End = outEnd
                    };
                    continue;
                }

                switch (entity.Type)
                {
                    case "builtin.percentage":
                        float.TryParse(entity.Resolution["value"].ToString().Replace("%", ""), out var parsedPercentage);
                        entityProps.Percentage = parsedPercentage / 100f;
                        break;

                    case "builtin.currency":
                        float.TryParse(entity.Resolution["value"].ToString(), out var parsedCurrency);
                        entityProps.Currency = parsedCurrency;
                        break;

                    //comparison operators. 
                    //TODO: Check for multiple operators (e.g. <=)
                    case "comparisonOperator::equal":
                        entityProps.ComparisonOperator = "equal";
                        break;

                    case "comparisonOperator::lessThan":
                        entityProps.ComparisonOperator = "lessThan";
                        break;
                    case "comparisonOperator::moreThan":
                        entityProps.ComparisonOperator = "moreThan";
                        break;

                    //custom ordinals
                    case "ordinalTense::last":
                        entityProps.OrdinalTense = "last";
                        break;

                    case "ordinalTense::first":
                        entityProps.OrdinalTense = "first";
                        break;
                }
            }

            return entityProps;
        }
    }
}
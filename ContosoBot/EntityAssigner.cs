﻿using System;
using System.Collections.Generic;
using System.Linq;
using ContosoData.Contollers;
using ContosoData.Model;
using Microsoft.Bot.Builder.Luis.Models;

namespace ContosoBot
{
    [Serializable]
    public class EntityAssigner
    {
        public EntityProps AssignEntities(LuisResult luisResult)
        {
            var entityProps = new EntityProps();

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
                    var outDateTimex = resolvedDict["timex"].ToString();
                    var outDateType = resolvedDict["type"].ToString();
                    DateTime outDateStart = new DateTime();
                    DateTime outDateEnd = new DateTime();


                    //parse datetime values
                    switch (outDateType)
                    {
                        case "date":
                            DateTime.TryParse(resolvedDict["value"].ToString(), out outDateStart);
                            break;
                        case "daterange":
                        case "datetimerange":
                            DateTime.TryParse(resolvedDict["start"].ToString(), out outDateStart);
                            DateTime.TryParse(resolvedDict["end"].ToString(), out outDateEnd);
                            break;
                    }


                    //set property to parsed values
                    entityProps.DateRange = new DateRange
                    {
                        Timex = outDateTimex,
                        Type = outDateType,
                        Start = outDateStart,
                        End = outDateEnd
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
                        float.TryParse(entity.Resolution["value"]?.ToString(), out var parsedCurrency);
                        entityProps.MoneyAmount = entityProps.MoneyAmount == 0 ? parsedCurrency : entityProps.MoneyAmount;

                            entityProps.MoneyCurrency.Add(
                                entity.Resolution["unit"]?
                                .ToString()
                                .ToLower()
                                );
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

                    case "account":
                        var trimmedEntity = entity.Score >= 0.7 ? entity.Entity.ToLower().Replace("account", "").Trim() : null;

                        if (string.IsNullOrEmpty(trimmedEntity))
                            break;

                        var accounts = AccountDataController.Accounts.ToList();
                        foreach (var account in accounts)
                        {
                            if (string.Equals(account.Name, trimmedEntity, StringComparison.CurrentCultureIgnoreCase) || 
                                string.Equals(account.Type, trimmedEntity, StringComparison.CurrentCultureIgnoreCase))
                            {
                                entityProps.Account = account;
                                break;
                            }
                        }
                        break;
                }
            }

            return entityProps;
        }
    }
}
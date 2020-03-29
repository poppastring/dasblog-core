using DasBlog.Web.Models.BlogViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace DasBlog.Web.Services
{
	public class RoutePermaLinkUnique
	{
		const string DATE_FORMAT = "yyyyMMdd";

		enum RouteType
		{
			IncludesDay,
			PostTitleOnly
		}

		RouteType routeType;

		public RoutePermaLinkUnique(bool includeDay)
		{
			this.routeType = includeDay ? RouteType.IncludesDay : RouteType.PostTitleOnly;
		}

		public Func<string, int, bool> IsSpecificPostRequested
		{
			get
			{
				IDictionary<RouteType, Func<string, int, bool>> isSpecificPostRequested = new Dictionary<RouteType, Func<string, int, bool>>
					{
						{RouteType.PostTitleOnly, (posttitle, day) => !string.IsNullOrEmpty(posttitle)},
						{RouteType.IncludesDay, (posttitle, day) => !string.IsNullOrEmpty(posttitle) && day != 0}
					};
				return isSpecificPostRequested[routeType];
			}
		}
		public Func<int, bool> IsValidDay
		{
			get
			{
				IDictionary<RouteType, Func<int, bool>> isValidDay = new Dictionary<RouteType, Func<int, bool>>
					{
						{RouteType.PostTitleOnly, day => true},
						{RouteType.IncludesDay, day => DateTime.TryParseExact(day.ToString(), DATE_FORMAT, null, DateTimeStyles.AdjustToUniversal, out _)}
					};
				return isValidDay[routeType];
			}
		}
		public Func<int, DateTime?> ConvertDayToDate
		{
			get
			{
				IDictionary<RouteType, Func<int, DateTime?>> convertDayToDate = new Dictionary<RouteType, Func<int, DateTime?>>
					{
						{RouteType.PostTitleOnly, day => null},
						{RouteType.IncludesDay, day => DateTime.ParseExact(day.ToString(), DATE_FORMAT
							, null, DateTimeStyles.AdjustToUniversal)}
					};
				return convertDayToDate[routeType];
			}
		}
		public Func<PostViewModel, DateTime?> SelectDate
		{
			get
			{
				IDictionary<RouteType, Func<PostViewModel, DateTime?>> convertDayToDate = new Dictionary<RouteType, Func<PostViewModel, DateTime?>>
					{
						{RouteType.PostTitleOnly, pvm => null},
						{RouteType.IncludesDay, pvm => pvm.CreatedDateTime}
					};
				return convertDayToDate[routeType];
			}
		}

	}
}

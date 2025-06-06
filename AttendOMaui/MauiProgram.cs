﻿#if DEBUG
using Microsoft.Extensions.Logging;
#endif

#if ANDROID
using Plugin.LocalNotification;
#endif

namespace AttendOMaui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		Dapper.SqlMapper.AddTypeHandler(new SqlDateOnlyTypeHandler());
		Dapper.SqlMapper.AddTypeHandler(new SqlTimeOnlyTypeHandler());

		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
#if ANDROID
			.UseMauiMaps()
			.UseLocalNotification(config =>
			{
				config.AddCategory(new NotificationCategory(NotificationCategoryType.Status)
				{
					ActionList = [new(100)
									{
											Title = "Navigate To Class",
											Android =
											{
												LaunchAppWhenTapped = true,
												IconName = { ResourceName = "attendances" }
											}
					}]
				});
			})
#endif
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}

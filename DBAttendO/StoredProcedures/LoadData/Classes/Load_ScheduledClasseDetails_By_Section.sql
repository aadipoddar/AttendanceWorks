﻿CREATE PROCEDURE [dbo].[Load_ScheduledClasseDetails_By_Section]
	@SectionId INT
AS
BEGIN
	SELECT
	*
	FROM
		[View_ScheduledClassDetails]
	WHERE
		SectionId = @SectionId;
END
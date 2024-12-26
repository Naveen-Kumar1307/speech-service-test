SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ASR Tables

CREATE TABLE [dbo].[Phoneme](
	[PhonemeId] [int] IDENTITY(1,1) NOT NULL,
	[Phoneme] [nvarchar](10) NOT NULL,
 CONSTRAINT [PK_Phoneme] PRIMARY KEY CLUSTERED 
(
	[PhonemeId] ASC
))
GO

CREATE TABLE [dbo].[RecentUserPhonemeScores](
	[RecentUserPhonemeScoresId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[PhonemeId] [int] NOT NULL,
	[PhonemeScore] [tinyint] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_RecentUserPhonemeScores] PRIMARY KEY CLUSTERED 
(
	[RecentUserPhonemeScoresId] ASC
))
GO

ALTER TABLE [dbo].[RecentUserPhonemeScores]  WITH CHECK ADD  CONSTRAINT [FK_RecentUserPhonemeScores_Phoneme] FOREIGN KEY([PhonemeId])
REFERENCES [dbo].[Phoneme] ([PhonemeId])
GO

ALTER TABLE [dbo].[RecentUserPhonemeScores] CHECK CONSTRAINT [FK_RecentUserPhonemeScores_Phoneme]
GO

CREATE NONCLUSTERED INDEX [IX_UserID_PhonemeID] ON [dbo].[RecentUserPhonemeScores] 
(
	[UserId] ASC
)
INCLUDE ( [PhonemeId])
GO


-- ASR Stored Procs

CREATE PROCEDURE [dbo].[usp_GE_Asr_GetRecentUserPhonemeScores]
	@UserId int, 
	@QualifiedThreshold int = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF @QualifiedThreshold IS NULL
		BEGIN
			SELECT rups.UserId, p.Phoneme, rups.PhonemeScore, rups.CreateDate 
			FROM RecentUserPhonemeScores rups WITH (NOLOCK)
			JOIN Phoneme p on p.PhonemeId = rups.PhonemeId
			WHERE UserId = @UserId
			ORDER BY CreateDate DESC
		END
	ELSE
		BEGIN
			SELECT rups.UserId, p.Phoneme, rups.PhonemeScore, rups.CreateDate
			FROM RecentUserPhonemeScores rups WITH (NOLOCK)
			JOIN Phoneme p on p.PhonemeId = rups.PhonemeId
			WHERE rups.UserId = @UserId
				AND p.PhonemeId IN (
					SELECT PhonemeId
					FROM RecentUserPhonemeScores WITH (NOLOCK)
					WHERE UserId = @UserId
					GROUP BY PhonemeId
					HAVING Count(PhonemeScore) >= @QualifiedThreshold
				)
			ORDER BY CreateDate DESC
		END
END

GO


CREATE PROCEDURE [dbo].[usp_GE_Asr_UpdateRecentUserPhonemeScores] 
	@UserId int, 
	@Phoneme nvarchar(10), 
	@PhonemeScore tinyint
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE 
		@MAX_NUM_REC int, 
		@NumRec int, 
		@LeastRecentId int,
		@PhonemeId int
	
	SET @MAX_NUM_REC = 8

	-- new phoneme? 
	SELECT @PhonemeId = PhonemeId 
	FROM Phoneme
	WHERE Phoneme = @Phoneme
	
	IF @PhonemeId IS NULL
		BEGIN
			INSERT INTO Phoneme (Phoneme) VALUES (@Phoneme)
			SET @PhonemeId = SCOPE_IDENTITY()
		END
		
	-- if less than @MAX_NUM_REC, simply insert
	SELECT @NumRec = (COUNT(RecentUserPhonemeScoresId)) 
	FROM RecentUserPhonemeScores WITH (NOLOCK)
	WHERE UserId = @UserId 
		AND PhonemeId = @PhonemeId
		
	IF (@NumRec < @MAX_NUM_REC) 
		BEGIN
			INSERT INTO RecentUserPhonemeScores (UserId, PhonemeId, PhonemeScore, CreateDate)
			VALUES (@UserId, @PhonemeId, @PhonemeScore, GETDATE())
		END
	ELSE	-- equal to or more than @MAX_NUM_REC, replace the least recent record. 
		BEGIN
			SET @LeastRecentId = (
				SELECT TOP 1 RecentUserPhonemeScoresId
				FROM RecentUserPhonemeScores WITH (NOLOCK)
				WHERE UserId = @UserId 
					AND PhonemeId = @PhonemeId
				ORDER BY CreateDate ASC )
				
			UPDATE RecentUserPhonemeScores
			SET PhonemeScore = @PhonemeScore, CreateDate = GETDATE()
			WHERE RecentUserPhonemeScoresId = @LeastRecentId
		END
END

GO


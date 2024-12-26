--select top 10 * from RecentUserPhonemeScores order by RecentUserPhonemeScoresId desc

--SELECT rups.UserId, p.Phoneme, rups.PhonemeScore, rups.CreateDate 
--FROM RecentUserPhonemeScores rups WITH (NOLOCK)
--JOIN Phoneme p on p.PhonemeId = rups.PhonemeId
--WHERE rups.UserId = 968590

DECLARE 
@LIMIT int,
@USER int

SET @LIMIT = 8
SET @USER = 25001

SELECT count(*) FROM RecentUserPhonemeScores WHERE UserId = @USER

SELECT UserId, p.Phoneme, PhonemeScore, CreateDate FROM (
	SELECT rups.UserId, rups.PhonemeId, rups.PhonemeScore, rups.CreateDate,
	RANK() OVER (PARTITION BY rups.PhonemeId ORDER BY rups.CreateDate DESC) rupsRank
	FROM RecentUserPhonemeScores rups WITH (NOLOCK)
	WHERE rups.UserId = @USER AND rups.PhonemeId IN (
		SELECT PhonemeId FROM RecentUserPhonemeScores WITH (NOLOCK)
		WHERE UserId = @USER GROUP BY PhonemeId
		HAVING Count(PhonemeScore) >= @LIMIT
	)
) scores
JOIN Phoneme p on p.PhonemeId = scores.PhonemeId
WHERE rupsRank <= @LIMIT
ORDER BY p.Phoneme, CreateDate DESC

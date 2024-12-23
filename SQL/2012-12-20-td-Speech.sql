-- INDEXES FOR SPEECH DB
-- Thomas Darmstandler 2012-12-20
---------------------



CREATE NONCLUSTERED INDEX ix_RecentUserPhonemeScores_UserId_PhonemeId 
                ON [Speech].[dbo].[RecentUserPhonemeScores] 
                                                ( [UserId], [PhonemeId] ) INCLUDE ([PhonemeScore], [CreateDate]);
                
                
                
CREATE NONCLUSTERED INDEX ix_RecentUserPhonemeScores_UserId 
                ON [Speech].[dbo].[RecentUserPhonemeScores] 
                                                ( [UserId] ) INCLUDE ([PhonemeId], [PhonemeScore], [CreateDate]);



CREATE NONCLUSTERED INDEX ix_RecentUserPhonemeScores_UserId_PhonemeId 
                ON [Speech].[dbo].[RecentUserPhonemeScores] 
                                ( [UserId], [PhonemeId] ) INCLUDE ([RecentUserPhonemeScoresId], [CreateDate]); 
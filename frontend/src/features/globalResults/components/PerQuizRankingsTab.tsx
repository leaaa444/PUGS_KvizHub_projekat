import React, { useState, useEffect, useMemo, useCallback } from 'react';
import Select from 'react-select'; 
import resultService from '../../../services/resultService';
import { useAuth } from '../../../context/AuthContext';

interface RankingEntry {
    username: string;
    userProfilePictureUrl: string;
    score: number;
    timeTaken: number;
    dateCompleted: number;
}

interface QuizRanking {
    quizId: number;
    quizName: string;
    topPlayers: RankingEntry[];
}

interface SelectOption {
    value: number;
    label: string;
}

interface PerQuizRankingsTabProps {
    startDate?: string;
    endDate?: string;
}

const PerQuizRankingsTab: React.FC<PerQuizRankingsTabProps> = ({ startDate, endDate }) => {
    const { user } = useAuth(); 
    const [rankings, setRankings] = useState<QuizRanking[]>([]);
    
    const [selectedQuiz, setSelectedQuiz] = useState<SelectOption | null>(null);
    const [loading, setLoading] = useState(true);

    const fetchRankings = useCallback(() => {
        setLoading(true);
        resultService.getAllRankings(startDate, endDate)
            .then(response => {
                const rankingsData: QuizRanking[] = response.data;
                const processedRankings = rankingsData.map(quizRanking => ({
                    ...quizRanking,
                    topPlayers: quizRanking.topPlayers.slice(0, 5) 
                }));

                setRankings(processedRankings);
            })
            .catch(error => console.error("Greška pri preuzimanju rang listi:", error))
            .finally(() => setLoading(false));
    }, [startDate, endDate]); 

    const displayedRankings = useMemo(() => {
        if (!selectedQuiz) {
            return rankings;
        }
        return rankings.filter(q => q.quizId === selectedQuiz.value);
    }, [rankings, selectedQuiz]);

    useEffect(() => {
        fetchRankings();
    }, [fetchRankings]);

    const quizOptions = useMemo(() => 
        rankings.map(q => ({ value: q.quizId, label: q.quizName })),
        [rankings]
    );   

    if (loading) {
        return <p>Učitavanje...</p>;
    }

    return (
        <>
            <div className="filters-container">
                <Select
                    options={quizOptions}
                    onChange={setSelectedQuiz}
                    value={selectedQuiz}
                    isClearable
                    placeholder="Pretraži i izaberi kviz..."
                    styles={customSelectStyles}
                    className="quiz-name-filter"
                />
            </div>
            <div className="rankings-grid">
                {displayedRankings.map(quizRanking => (
                    <div key={quizRanking.quizId} className="quiz-ranking-card">
                        <h3>{quizRanking.quizName}</h3>
                        <ol className="players-list">
                            {quizRanking.topPlayers.map((player, index) => (
                                <li 
                                    key={player.username} 
                                    className={user?.username === player.username ? 'highlighted-player' : ''}
                                >
                                    <span className="player-rank">{index + 1}.</span>
                                    <img 
                                        src={`${process.env.REACT_APP_API_BASE_URL}${player.userProfilePictureUrl}`} 
                                        alt={player.username} 
                                        className="player-avatar"
                                    />
                                    <span className="player-name">{player.username}</span>
                                    <div className="player-stats">
                                        <span className="player-score">{player.score.toLocaleString('sr-RS')} bodova</span>
                                        <span className="player-time">{Math.floor(player.timeTaken / 60)}m {player.timeTaken % 60}s</span>
                                    </div>
                                </li>
                            ))}
                        </ol>
                        {quizRanking.topPlayers.length === 0 && (
                            <p className="no-results-msg">Još nema rezultata za ovaj kviz.</p>
                        )}
                    </div>
                ))}
            </div>
        </>
    );
};

const customSelectStyles = {
    control: (provided: any) => ({
        ...provided,
        backgroundColor: 'var(--input-bg)',
        borderColor: 'var(--foreground)',
        boxShadow: 'none',
        '&:hover': { borderColor: 'var(--foreground)' }
    }),
    menu: (provided: any) => ({
        ...provided,
        backgroundColor: 'var(--button-color)',
    }),
    option: (provided: any, state: { isFocused: any; }) => ({
        ...provided,
        backgroundColor: state.isFocused ? 'var(--background)' : 'var(--button-color)',
        color: 'var(--text-color)',
        '&:active': { backgroundColor: 'var(--background)' }
    }),
    singleValue: (provided: any) => ({ ...provided, color: 'var(--input-text)' }),
    input: (provided: any) => ({ ...provided, color: 'var(--input-text)' }),
    placeholder: (provided: any) => ({ ...provided, color: 'rgba(238, 215, 197, 0.6)'})
};


export default PerQuizRankingsTab;
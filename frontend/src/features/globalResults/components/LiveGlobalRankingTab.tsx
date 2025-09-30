import React, { useState, useEffect, useCallback } from 'react';
import liveResultService from '../../../services/liveResultService';
import { useAuth } from '../../../context/AuthContext';

interface LiveGlobalRanking {
    position: number;
    username: string;
    userProfilePictureUrl: string;
    averageScore: number; 
    arenasPlayed: number;
}

interface LiveGlobalRankingTabProps {
    startDate?: string;
    endDate?: string;
}

const LiveGlobalRankingTab: React.FC<LiveGlobalRankingTabProps> = ({ startDate, endDate }) => { 
    const { user } = useAuth();
    const [rankings, setRankings] = useState<LiveGlobalRanking[]>([]);
    const [loading, setLoading] = useState(true);

    const fetchRanking = useCallback(() => {
        setLoading(true);
        liveResultService.getGlobalRanking(startDate, endDate)
            .then(response => {
                setRankings(response.data);
            })
            .catch(error => {
                console.error("Greška pri preuzimanju live globalne rang liste:", error);
            })
            .finally(() => {
                setLoading(false);
            });
    }, [startDate, endDate]); 

    useEffect(() => {
        fetchRanking();
    }, [fetchRanking]);

    if (loading) {
        return <p>Učitavanje rang liste...</p>;
    }

    return (
        <>
            {rankings.length > 0 ? (
                <table className="global-ranking-table">
                    <thead>
                        <tr>
                            <th>Poz.</th>
                            <th colSpan={2}>Igrač</th>
                            <th>Br. Arena</th>
                            <th>Prosečan Skor</th>
                        </tr>
                    </thead>
                    <tbody>
                        {rankings.map(player => (
                            <tr key={player.position} className={user?.username === player.username ? 'highlighted-row' : ''}>
                                <td className="rank-position">{player.position}</td>
                                <td className="rank-avatar">
                                    <img 
                                        src={`${process.env.REACT_APP_API_BASE_URL}${player.userProfilePictureUrl}`} 
                                        alt={player.username} 
                                    />
                                </td>
                                <td className="rank-username">{player.username}</td>
                                <td>{player.arenasPlayed}</td>
                                <td className="rank-score">{player.averageScore.toFixed(2)}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            ) : (
                <p>Nema rezultata za prikaz.</p>
            )}
        </>
    );
};

export default LiveGlobalRankingTab;
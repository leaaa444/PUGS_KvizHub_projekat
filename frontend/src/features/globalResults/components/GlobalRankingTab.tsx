import React, { useState, useEffect } from 'react';
import resultService from '../../../services/resultService';
import { useAuth } from '../../../context/AuthContext';

interface GlobalRanking {
    position: number;
    username: string;
    userProfilePictureUrl: string;
    globalScore: number;
    quizzesPlayed: number;
}

const GlobalRankingTab: React.FC = () => {
    const { user } = useAuth();
    const [rankings, setRankings] = useState<GlobalRanking[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        resultService.getGlobalRanking()
            .then(response => {
                setRankings(response.data);
            })
            .catch(error => {
                console.error("Greška pri preuzimanju globalne rang liste:", error);
            })
            .finally(() => {
                setLoading(false);
            });
    }, []);

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
                            <th>Br. Kvizova</th>
                            <th>Ukupan Skor</th>
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
                                <td>{player.quizzesPlayed}</td>
                                <td className="rank-score">{player.globalScore.toFixed(2)}</td>
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

export default GlobalRankingTab;
import React from 'react';
import { Player } from '../types'; 
import { useAuth } from '../../../context/AuthContext'; 

interface LiveLeaderboardProps {
    title: string;
    players: Player[];
    containerClassName?: string;
}

const LiveLeaderboard: React.FC<LiveLeaderboardProps> = ({ title, players, containerClassName }) => {
    const { user } = useAuth(); 

    return (
        <div className={containerClassName || 'leaderboard'}>
            <h3>{title}</h3>
            <ul>
                {[...players]
                    .sort((a, b) => b.score - a.score)
                    .map((p, index) => {
                        const isCurrentUser = p.username === user?.username;
                        const userInitial = p.username.charAt(0).toUpperCase();
                        
                        const API_BASE_URL = process.env.REACT_APP_API_BASE_URL;
                        let fullImageUrl = p.imageUrl;
                        if (p.imageUrl && !p.imageUrl.startsWith('http')) {
                            fullImageUrl = `${API_BASE_URL}${p.imageUrl}`;
                        }

                        return (
                            <li 
                                key={p.username} 
                                className={`player-item ${isCurrentUser ? 'highlight' : ''} ${p.isDisconnected ? 'disconnected' : ''}`}
                            >
                                <div className="player-info">
                                    <span className="player-rank">{index + 1}.</span>
                                    
                                    {fullImageUrl ? (
                                        <img src={fullImageUrl} alt={p.username} className="player-avatar-img" />
                                    ) : (
                                        <div className="player-avatar">{userInitial}</div>
                                    )}

                                    <span className="player-name">{p.username}</span>
                                </div>
                                <span className="player-score">{p.score} poena</span>
                            </li>
                        );
                    })}
            </ul>
        </div>
    );
};

export default LiveLeaderboard;
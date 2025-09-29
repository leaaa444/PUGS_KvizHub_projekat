import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAuth } from '../../../context/AuthContext';
import {
    enterLobby,
    startGame,
    subscribeToRoomUpdates,
    onError,
    onGameStarted,
    onHostDisconnected,
    onRoomClosed
} from '../../../services/signalrService';
import { GameRoom } from '../types';
import './LobbyPage.css'


const LobbyPage = () => {
    const { roomCode } = useParams<{ roomCode: string }>();
    const { user } = useAuth();
    const [room, setRoom] = useState<GameRoom | null>(null);
    const navigate = useNavigate();

    useEffect(() => {
        if (!roomCode) return;

        // Handler-i za događaje
        const handleRoomUpdate = (updatedRoom: GameRoom) => setRoom(updatedRoom);
        const handleError = (message: string) => { alert(`Greška: ${message}`); navigate('/kvizovi'); };
        const handleHostDisconnected = (message: string) => alert(message); 
        const handleGameStarted = () => navigate(`/live-arena/game/${roomCode}`); 
        const handleRoomClosed = (message: string) => { alert(message); navigate('/kvizovi'); };

        // Pretplata na događaje
        const unsubscribeRoomUpdates = subscribeToRoomUpdates(handleRoomUpdate);
        const unsubscribeError = onError(handleError);
        const unsubscribeHostDisconnected = onHostDisconnected(handleHostDisconnected);
        const unsubscribeGameStarted = onGameStarted(handleGameStarted);
        const unsubscribeRoomClosed = onRoomClosed(handleRoomClosed);
        
        // Glavna funkcija za ulazak u lobi
        enterLobby(roomCode).catch(err => {
            console.error(err);
            alert("Nije moguće ući u sobu.");
            navigate('/kvizovi');
        });

        // Cleanup funkcija
        return () => {
            unsubscribeRoomUpdates();
            unsubscribeError();
            unsubscribeHostDisconnected();
            unsubscribeGameStarted();
            unsubscribeRoomClosed();
        };
    }, [roomCode, navigate]);

    const handleStartGame = () => {
        if (roomCode) {
            startGame(roomCode);
        }
    };


    const isHost = user?.username === room?.hostUsername;

    if (!room) {
        return <p>Povezivanje sa sobom {roomCode}...</p>;
    }

   return (
        <div className="lobby-container">
        
        {/* Zaglavlje sa kodom sobe */}
        <div className="lobby-header">
            <h3>Kod Sobe:</h3>
            <div className="room-code-wrapper">
                <span className="room-code-display">{room.roomCode}</span>
                <button 
                    className="copy-btn" 
                    onClick={() => navigator.clipboard.writeText(room.roomCode)}
                >
                    Kopiraj
                </button>
            </div>
        </div>

        <div className="host-info">
            <h4>DOMAĆIN</h4>
            <p>
                👑 {room.hostUsername}
                {isHost && <span className="player-tag"> (Vi)</span>}
            </p>
        </div>

        {/* Lista igrača sada prikazuje samo učesnike */}
        <div className="player-list-wrapper">
            {/* Brojač sada prikazuje tačan broj igrača */}
            <h3>Igrači ({room.players.length}):</h3>
            <ul className="player-list">
                {room.players.length > 0 ? (
                    room.players.map(p => (
                        <li key={p.username} className="player-item">
                            <span>{p.username}</span>
                        </li>
                    ))
                ) : (
                    <p className="wait-message">Čekaju se igrači...</p>
                )}
            </ul>
        </div>
        
        {/* Akcije i dugmići */}
        <div className="lobby-actions">
            {isHost && room.status === 'Lobby' && (
                <button className="start-game-btn" onClick={handleStartGame}>
                    Pokreni Kviz!
                </button>
            )}
            
            {!isHost && room.status === 'Lobby' && (
                <p className="wait-message">Sačekajte da domaćin ({room.hostUsername}) pokrene kviz...</p>
            )}

            {room.status === 'Finished' && (
                 <p className="wait-message">Kviz je završen! Domaćin može da pokrene novu igru.</p>
            )}
        </div>

    </div>
    );
};

export default LobbyPage;
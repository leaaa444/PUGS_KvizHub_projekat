import React, { useState, useEffect } from 'react';
import liveResultService from '../../../services/liveResultService';
import { GameRoom } from '../../live-quiz/types'; // Importuj GameRoom tip
import LiveLeaderboard from '../../live-quiz/game/LiveLeaderboard'; 

interface MyFinishedArena {
    roomCode: string;
    quizName: string;
    finishedAt: string;
    yourScore: number;
    yourRank: number;
    participantCount: number;
}

const MyLiveArenasTab: React.FC = () => {
    const [arenas, setArenas] = useState<MyFinishedArena[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        liveResultService.getMyFinishedArenas().then(response => setArenas(response.data));
    }, []);

    const [isModalOpen, setIsModalOpen] = useState(false);
    const [modalLoading, setModalLoading] = useState(false);
    const [selectedArenaDetails, setSelectedArenaDetails] = useState<GameRoom | null>(null);

    useEffect(() => {
        liveResultService.getMyFinishedArenas()
            .then(response => {
                setArenas(response.data);
            })
            .catch(error => console.error("Greška pri preuzimanju mojih arena:", error))
            .finally(() => setLoading(false));
    }, []);

    const handleOpenDetailsModal = (roomCode: string) => {
        setIsModalOpen(true);
        setModalLoading(true);
        liveResultService.getArenaDetails(roomCode)
            .then(response => {
                setSelectedArenaDetails(response.data);
            })
            .catch(error => {
                console.error("Greška pri preuzimanju detalja arene:", error);
                setIsModalOpen(false); // Zatvori modal u slučaju greške
            })
            .finally(() => setModalLoading(false));
    };

    const handleCloseModal = () => {
        setIsModalOpen(false);
        setSelectedArenaDetails(null);
    };

    if (loading) {
        return <p>Učitavanje rezultata...</p>;
    }

   return (
        <>
            {arenas.length > 0 ? (
                <table className="results-table">
                    <thead>
                        <tr>
                            <th>Naziv Kviza</th>
                            <th>Datum</th>
                            <th>Tvoj Skor</th>
                            <th>Tvoj Rank</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        {arenas.map(arena => (
                            <tr key={arena.roomCode}>
                                <td>{arena.quizName}</td>
                                <td>{new Date(arena.finishedAt).toLocaleDateString('sr-RS')}</td>
                                <td>{arena.yourScore.toLocaleString('sr-RS')}</td>
                                <td>{arena.yourRank}. / {arena.participantCount}</td>
                                <td>
                                    <button onClick={() => handleOpenDetailsModal(arena.roomCode)} className="btn-details">
                                        Detalji Partije
                                    </button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            ) : (
                <p>Niste učestvovali ni u jednoj Live Areni.</p>
            )}

            {isModalOpen && (
                <div className="custom-modal-overlay" onClick={handleCloseModal}>
                    <div className="custom-modal-content" onClick={(e) => e.stopPropagation()}>
                        <button onClick={handleCloseModal} className="custom-modal-close-btn">&times;</button>
                        
                        <div className="arena-details-modal">
                            {modalLoading && <p>Učitavanje rezultata...</p>}
                            
                            {!modalLoading && selectedArenaDetails && (
                                <>
                                    <h2 className="details-title">Rezultati Arene</h2>
                                    <div className="details-subtitle">
                                        <span>Kviz: <strong>{selectedArenaDetails.quizName || 'Nepoznat Kviz'}</strong></span>
                                        <span>Kod sobe: <strong>{selectedArenaDetails.roomCode}</strong></span>
                                    </div>

                                    <LiveLeaderboard
                                        title="Finalni Poredak"
                                        players={selectedArenaDetails.players}
                                        containerClassName="final-leaderboard-modal"
                                    />
                                </>
                            )}
                        </div>

                    </div>
                </div>
            )}
        </>
    );
};

export default MyLiveArenasTab;
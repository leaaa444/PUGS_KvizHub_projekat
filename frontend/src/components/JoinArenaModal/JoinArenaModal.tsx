import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import Modal from '../Modal/Modal';
// Više nam ne treba ništa iz signalrService-a ovde

interface JoinArenaModalProps {
    isOpen: boolean;
    onClose: () => void;
}

const JoinArenaModal: React.FC<JoinArenaModalProps> = ({ isOpen, onClose }) => {
    const navigate = useNavigate();
    const [roomCode, setRoomCode] = useState('');
    const [error, setError] = useState(''); // Možemo ostaviti za neku buduću validaciju

    // Resetuj polje kada se modal otvori
    useEffect(() => {
        if (isOpen) {
            setRoomCode('');
            setError('');
        }
    }, [isOpen]);

    const handleJoin = (e: React.FormEvent) => {
        e.preventDefault();
        
        // Jednostavna provera da li je kod unet
        if (!roomCode.trim()) {
            setError('Morate uneti kod sobe.');
            return;
        }

        const formattedRoomCode = roomCode.trim().toUpperCase();
        
        // 1. Zatvori modal
        onClose();
        
        // 2. Samo navigiraj na Lobby stranicu sa unetim kodom
        navigate(`/live-arena/lobby/${formattedRoomCode}`);
    };

    return (
        <Modal isOpen={isOpen} onClose={onClose}>
            <div>
                <h2>Pridruži se Areni</h2>
                <form onSubmit={handleJoin}>
                    {/* Polje za username nam više ne treba */}
                    <input
                        type="text"
                        value={roomCode}
                        onChange={e => setRoomCode(e.target.value)}
                        placeholder="Unesi kod sobe"
                        autoFocus
                        required
                        maxLength={5} // Kod je 5 karaktera
                    />
                    <button type="submit">Pridruži se</button>
                    {error && <p style={{ color: 'red' }}>{error}</p>}
                </form>
            </div>
        </Modal>
    );
};

export default JoinArenaModal;
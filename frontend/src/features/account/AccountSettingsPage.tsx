import React, { useState, useEffect } from 'react';
import accountService from '../../services/accountService';
import { useAuth } from '../../context/AuthContext';
import './AccountSettingsPage.css';

const AccountSettingsPage = () => {
    const { user, setUser } = useAuth();
    
    const [profileData, setProfileData] = useState({ username: '', email: '' });
    const [passwordData, setPasswordData] = useState({ currentPassword: '', newPassword: '', confirmNewPassword: '' });
    const [selectedFile, setSelectedFile] = useState<File | null>(null);
    const [message, setMessage] = useState<{ type: 'success' | 'error'; text: string } | null>(null);
    const [isSaving, setIsSaving] = useState(false);

    useEffect(() => {
        // Popunjavamo formu sa podacima iz AuthContexta kada se komponenta učita
        if (user) {
            setProfileData({ username: user.username, email: user.email });
        }
    }, [user]);

    const handleProfileChange = (e: React.ChangeEvent<HTMLInputElement>) => setProfileData({ ...profileData, [e.target.name]: e.target.value });
    const handlePasswordChange = (e: React.ChangeEvent<HTMLInputElement>) => setPasswordData({ ...passwordData, [e.target.name]: e.target.value });
    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (e.target.files) setSelectedFile(e.target.files[0]);
    };

    const handleProfileSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setIsSaving(true);
        setMessage(null);
        try {
            await accountService.updateProfile(profileData);
            setUser({ ...user!, username: profileData.username, email: profileData.email });
            setMessage({ type: 'success', text: 'Osnovni podaci su uspešno ažurirani!' });
        } catch (err: any) {
            setMessage({ type: 'error', text: err.response?.data?.message || 'Greška pri ažuriranju profila.' });
        } finally {
            setIsSaving(false);
        }
    };
    
    const handlePasswordSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (passwordData.newPassword !== passwordData.confirmNewPassword) {
            setMessage({ type: 'error', text: 'Nove lozinke se ne poklapaju.' });
            return;
        }
        setIsSaving(true);
        setMessage(null);
        try {
            await accountService.changePassword({ currentPassword: passwordData.currentPassword, newPassword: passwordData.newPassword });
            setMessage({ type: 'success', text: 'Lozinka je uspešno promenjena!' });
            setPasswordData({ currentPassword: '', newPassword: '', confirmNewPassword: '' });
        } catch (err: any) {
            setMessage({ type: 'error', text: err.response?.data?.message || 'Greška pri promeni lozinke.' });
        } finally {
            setIsSaving(false);
        }
    };

    const handlePictureSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!selectedFile) return;
        setIsSaving(true);
        setMessage(null);
        try {
            const response = await accountService.updateProfilePicture(selectedFile);
            setUser({ ...user!, profilePictureUrl: response.data.newUrl });
            setMessage({ type: 'success', text: 'Profilna slika je uspešno promenjena!' });
            setSelectedFile(null); // Resetuj selektovani fajl
        } catch (err: any) {
            setMessage({ type: 'error', text: 'Greška pri upload-u slike.' });
        } finally {
            setIsSaving(false);
        }
    };

    if (!user) return <div className="settings-page">Učitavanje...</div>;

    return (
        <div className="settings-page">
            <h2>Podešavanja Naloga</h2>
            {message && <div className={`message ${message.type}`}>{message.text}</div>}

            <form onSubmit={handlePictureSubmit} className="settings-section">
                <h3>Profilna slika</h3>
                <img 
                    src={`${process.env.REACT_APP_API_BASE_URL}${user.profilePictureUrl}`} 
                    alt="Profilna" 
                    className="profile-avatar" 
                />
                <input type="file" accept="image/png, image/jpeg" onChange={handleFileChange} />
                <button type="submit" disabled={!selectedFile || isSaving}>
                    {isSaving ? 'Slanje...' : 'Promeni sliku'}
                </button>
            </form>

            <form onSubmit={handleProfileSubmit} className="settings-section">
                <h3>Osnovni podaci</h3>
                <label htmlFor="username">Korisničko ime</label>
                <input id="username" type="text" name="username" value={profileData.username} onChange={handleProfileChange} required />
                <label htmlFor="email">Email</label>
                <input id="email" type="email" name="email" value={profileData.email} onChange={handleProfileChange} required />
                <button type="submit" disabled={isSaving}>
                    {isSaving ? 'Čuvanje...' : 'Sačuvaj izmene'}
                </button>
            </form>
            
            <form onSubmit={handlePasswordSubmit} className="settings-section">
                <h3>Promena lozinke</h3>
                <label htmlFor="currentPassword">Trenutna lozinka</label>
                <input id="currentPassword" type="password" name="currentPassword" value={passwordData.currentPassword} onChange={handlePasswordChange} required />
                <label htmlFor="newPassword">Nova lozinka</label>
                <input id="newPassword" type="password" name="newPassword" value={passwordData.newPassword} onChange={handlePasswordChange} required />
                <label htmlFor="confirmNewPassword">Potvrdi novu lozinku</label>
                <input id="confirmNewPassword" type="password" name="confirmNewPassword" value={passwordData.confirmNewPassword} onChange={handlePasswordChange} required />
                <button type="submit" disabled={isSaving}>
                    {isSaving ? 'Menjanje...' : 'Promeni lozinku'}
                </button>
            </form>
        </div>
    );
};

export default AccountSettingsPage;
const toastElement = document.getElementById('toastContainer');
const toast = toastElement ? new bootstrap.Toast(toastElement) : null;
const apiBase = '/api';
let roomTypes = [];
let rooms = [];
let guests = [];
let bookings = [];

window.addEventListener('DOMContentLoaded', () => {
    refreshAll();
});

async function refreshAll() {
    await Promise.all([loadRoomTypes(), loadRooms(), loadGuests(), loadBookings()]);
}

function showToast(message) {
    if (!toastElement) return;
    document.getElementById('toastMessage').textContent = message;
    toast.show();
}

function filterTable(tableId, value) {
    const filter = value.toLowerCase();
    document.querySelectorAll(`#${tableId} tbody tr`).forEach(row => {
        row.style.display = row.textContent.toLowerCase().includes(filter) ? '' : 'none';
    });
}

async function loadRoomTypes() {
    const response = await fetch(`${apiBase}/roomtypes`);
    roomTypes = await response.json();
    const tbody = document.querySelector('#roomTypesTable tbody');
    tbody.innerHTML = '';
    roomTypes.forEach(rt => {
        const row = document.createElement('tr');
        row.innerHTML = `<td>${rt.name}</td><td>${rt.capacity}</td><td>${rt.pricePerNight.toFixed(2)}</td><td><button class="btn btn-sm btn-primary me-2" onclick="showRoomTypeModal(${rt.id})">Edit</button><button class="btn btn-sm btn-danger" onclick="deleteRoomType(${rt.id})">Delete</button></td>`;
        tbody.append(row);
    });
    fillRoomTypeSelect();
}

async function loadRooms() {
    const response = await fetch(`${apiBase}/rooms`);
    rooms = await response.json();
    const tbody = document.querySelector('#roomsTable tbody');
    tbody.innerHTML = '';
    rooms.forEach(room => {
        const row = document.createElement('tr');
        row.innerHTML = `<td>${room.number}</td><td>${room.floor}</td><td>${room.isAvailable ? 'Yes' : 'No'}</td><td>${room.roomTypeName}</td><td>${room.pricePerNight.toFixed(2)}</td><td><button class="btn btn-sm btn-primary me-2" onclick="showRoomModal(${room.id})">Edit</button><button class="btn btn-sm btn-danger" onclick="deleteRoom(${room.id})">Delete</button></td>`;
        tbody.append(row);
    });
}

async function loadGuests() {
    const response = await fetch(`${apiBase}/guests`);
    guests = await response.json();
    const tbody = document.querySelector('#guestsTable tbody');
    tbody.innerHTML = '';
    guests.forEach(guest => {
        const row = document.createElement('tr');
        row.innerHTML = `<td>${guest.firstName} ${guest.lastName}</td><td>${guest.email}</td><td>${guest.phone}</td><td><button class="btn btn-sm btn-primary me-2" onclick="showGuestModal(${guest.id})">Edit</button><button class="btn btn-sm btn-danger" onclick="deleteGuest(${guest.id})">Delete</button></td>`;
        tbody.append(row);
    });
    fillBookingGuestSelect();
}

async function loadBookings() {
    const response = await fetch(`${apiBase}/bookings`);
    bookings = await response.json();
    const tbody = document.querySelector('#bookingsTable tbody');
    tbody.innerHTML = '';
    bookings.forEach(booking => {
        const row = document.createElement('tr');
        row.innerHTML = `<td>${booking.guestName}</td><td>${booking.roomNumber}</td><td>${new Date(booking.checkIn).toLocaleDateString()}</td><td>${new Date(booking.checkOut).toLocaleDateString()}</td><td>${booking.totalPrice.toFixed(2)}</td><td><button class="btn btn-sm btn-primary me-2" onclick="showBookingModal(${booking.id})">Edit</button><button class="btn btn-sm btn-danger" onclick="deleteBooking(${booking.id})">Delete</button></td>`;
        tbody.append(row);
    });
    fillBookingRoomSelect();
}

function fillRoomTypeSelect() {
    const select = document.getElementById('roomTypeSelect');
    select.innerHTML = '<option value="">Select room type</option>';
    roomTypes.forEach(rt => {
        const option = document.createElement('option');
        option.value = rt.id;
        option.textContent = `${rt.name} (${rt.capacity} guests)`;
        select.append(option);
    });
}

function fillBookingGuestSelect() {
    const select = document.getElementById('bookingGuest');
    select.innerHTML = '<option value="">Select guest</option>';
    guests.forEach(g => {
        const option = document.createElement('option');
        option.value = g.id;
        option.textContent = `${g.firstName} ${g.lastName}`;
        select.append(option);
    });
}

function fillBookingRoomSelect() {
    const select = document.getElementById('bookingRoom');
    select.innerHTML = '<option value="">Select room</option>';
    rooms.forEach(room => {
        const option = document.createElement('option');
        option.value = room.id;
        option.textContent = `${room.number} (${room.roomTypeName})`;
        select.append(option);
    });
}

function showRoomTypeModal(id) {
    const modal = new bootstrap.Modal(document.getElementById('roomTypeModal'));
    document.getElementById('roomTypeForm').reset();
    document.getElementById('roomTypeId').value = '';
    document.getElementById('roomTypeModalLabel').textContent = id ? 'Edit Room Type' : 'Add Room Type';
    if (id) {
        const item = roomTypes.find(x => x.id === id);
        if (item) {
            document.getElementById('roomTypeId').value = item.id;
            document.getElementById('roomTypeName').value = item.name;
            document.getElementById('roomTypeCapacity').value = item.capacity;
            document.getElementById('roomTypePrice').value = item.pricePerNight;
        }
    }
    modal.show();
}

async function saveRoomType(event) {
    event.preventDefault();
    const id = document.getElementById('roomTypeId').value;
    const payload = {
        name: document.getElementById('roomTypeName').value,
        capacity: Number(document.getElementById('roomTypeCapacity').value),
        pricePerNight: Number(document.getElementById('roomTypePrice').value)
    };

    const method = id ? 'PUT' : 'POST';
    const url = `${apiBase}/roomtypes${id ? '/' + id : ''}`;
    const response = await fetch(url, {
        method,
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify(payload)
    });

    if (response.ok) {
        showToast('Room type saved');
        bootstrap.Modal.getInstance(document.getElementById('roomTypeModal')).hide();
        loadRoomTypes();
    } else {
        const error = await parseError(response);
        showToast(error);
    }
}

async function deleteRoomType(id) {
    if (!confirm('Delete this room type?')) return;
    const response = await fetch(`${apiBase}/roomtypes/${id}`, { method: 'DELETE' });
    if (response.ok) {
        showToast('Room type deleted');
        loadRoomTypes();
    } else {
        showToast(await parseError(response));
    }
}

function showRoomModal(id) {
    const modal = new bootstrap.Modal(document.getElementById('roomModal'));
    document.getElementById('roomForm').reset();
    document.getElementById('roomId').value = '';
    document.getElementById('roomModalLabel').textContent = id ? 'Edit Room' : 'Add Room';
    fillRoomTypeSelect();
    if (id) {
        const item = rooms.find(x => x.id === id);
        if (item) {
            document.getElementById('roomId').value = item.id;
            document.getElementById('roomNumber').value = item.number;
            document.getElementById('roomFloor').value = item.floor;
            document.getElementById('roomTypeSelect').value = item.roomTypeId;
            document.getElementById('roomAvailable').checked = item.isAvailable;
        }
    }
    modal.show();
}

async function saveRoom(event) {
    event.preventDefault();
    const id = document.getElementById('roomId').value;
    const payload = {
        number: document.getElementById('roomNumber').value,
        floor: Number(document.getElementById('roomFloor').value),
        isAvailable: document.getElementById('roomAvailable').checked,
        roomTypeId: Number(document.getElementById('roomTypeSelect').value)
    };
    const response = await fetch(`${apiBase}/rooms${id ? '/' + id : ''}`, {
        method: id ? 'PUT' : 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify(payload)
    });
    if (response.ok) {
        showToast('Room saved');
        bootstrap.Modal.getInstance(document.getElementById('roomModal')).hide();
        await Promise.all([loadRooms(), loadBookings()]);
    } else {
        showToast(await parseError(response));
    }
}

async function deleteRoom(id) {
    if (!confirm('Delete this room?')) return;
    const response = await fetch(`${apiBase}/rooms/${id}`, { method: 'DELETE' });
    if (response.ok) {
        showToast('Room deleted');
        loadRooms();
    } else {
        showToast(await parseError(response));
    }
}

function showGuestModal(id) {
    const modal = new bootstrap.Modal(document.getElementById('guestModal'));
    document.getElementById('guestForm').reset();
    document.getElementById('guestId').value = '';
    document.getElementById('guestModalLabel').textContent = id ? 'Edit Guest' : 'Add Guest';
    if (id) {
        const item = guests.find(x => x.id === id);
        if (item) {
            document.getElementById('guestId').value = item.id;
            document.getElementById('guestFirstName').value = item.firstName;
            document.getElementById('guestLastName').value = item.lastName;
            document.getElementById('guestEmail').value = item.email;
            document.getElementById('guestPhone').value = item.phone;
        }
    }
    modal.show();
}

async function saveGuest(event) {
    event.preventDefault();
    const id = document.getElementById('guestId').value;
    const payload = {
        firstName: document.getElementById('guestFirstName').value,
        lastName: document.getElementById('guestLastName').value,
        email: document.getElementById('guestEmail').value,
        phone: document.getElementById('guestPhone').value
    };
    const response = await fetch(`${apiBase}/guests${id ? '/' + id : ''}`, {
        method: id ? 'PUT' : 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify(payload)
    });
    if (response.ok) {
        showToast('Guest saved');
        bootstrap.Modal.getInstance(document.getElementById('guestModal')).hide();
        await Promise.all([loadGuests(), loadBookings()]);
    } else {
        showToast(await parseError(response));
    }
}

async function deleteGuest(id) {
    if (!confirm('Delete this guest?')) return;
    const response = await fetch(`${apiBase}/guests/${id}`, { method: 'DELETE' });
    if (response.ok) {
        showToast('Guest deleted');
        loadGuests();
    } else {
        showToast(await parseError(response));
    }
}

function showBookingModal(id) {
    const modal = new bootstrap.Modal(document.getElementById('bookingModal'));
    document.getElementById('bookingForm').reset();
    document.getElementById('bookingId').value = '';
    document.getElementById('bookingModalLabel').textContent = id ? 'Edit Booking' : 'Add Booking';
    fillBookingGuestSelect();
    fillBookingRoomSelect();
    if (id) {
        const item = bookings.find(x => x.id === id);
        if (item) {
            document.getElementById('bookingId').value = item.id;
            document.getElementById('bookingGuest').value = item.guestId;
            document.getElementById('bookingRoom').value = item.roomId;
            document.getElementById('bookingCheckIn').value = item.checkIn.split('T')[0];
            document.getElementById('bookingCheckOut').value = item.checkOut.split('T')[0];
        }
    }
    modal.show();
}

async function saveBooking(event) {
    event.preventDefault();
    const id = document.getElementById('bookingId').value;
    const payload = {
        guestId: Number(document.getElementById('bookingGuest').value),
        roomId: Number(document.getElementById('bookingRoom').value),
        checkIn: document.getElementById('bookingCheckIn').value,
        checkOut: document.getElementById('bookingCheckOut').value
    };
    const response = await fetch(`${apiBase}/bookings${id ? '/' + id : ''}`, {
        method: id ? 'PUT' : 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify(payload)
    });
    if (response.ok) {
        showToast('Booking saved');
        bootstrap.Modal.getInstance(document.getElementById('bookingModal')).hide();
        await loadBookings();
    } else {
        showToast(await parseError(response));
    }
}

async function deleteBooking(id) {
    if (!confirm('Delete this booking?')) return;
    const response = await fetch(`${apiBase}/bookings/${id}`, { method: 'DELETE' });
    if (response.ok) {
        showToast('Booking deleted');
        loadBookings();
    } else {
        showToast(await parseError(response));
    }
}

async function parseError(response) {
    try {
        const contentType = response.headers.get('Content-Type') || '';
        if (contentType.includes('application/json')) {
            const data = await response.json();
            return data.title || data.error || data.message || JSON.stringify(data);
        }
        return await response.text();
    } catch {
        return 'An unexpected error occurred';
    }
}

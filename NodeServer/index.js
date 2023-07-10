const express = require('express');
const http = require('http');
const { Server } = require('socket.io');
const cors = require('cors');

const app = express();
app.use(express.json());
app.use(cors());

var exSocket;

app.get('/', async (request, response) => {
  console.log("new change");
  exSocket.emit("new_data", `${request.headers.id}: ${request.headers.name} ${request.headers.surname}`)
  response.send("good");
});

// Server and sockets
const server = http.createServer(app);
const io = new Server(server, {
  cors: {
    origin: '*',
    methods: ['GET', 'POST'],
  },
});
io.on('connection', (socket) => {
  exSocket = socket;
  console.log('New websocket user connected');
});

// Listen for webhook events //
server.listen(process.env.PORT || 3370, () => console.log('server is listening'));

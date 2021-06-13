import { Button, Card, Grid, makeStyles, TextField } from "@material-ui/core";
import { gql, useMutation, useQuery, useSubscription } from "@apollo/client";
import { useEffect, useState } from "react";

const CREATE_MESSAGE = gql`
	mutation CreateMessage($body: String!) {
		createMessage(input: { body: $body }) {
			id
			body
			createdAt
		}
	}
`;

const MESSAGES = gql`
	query Messages {
		messages {
			id
			body
			createdAt
		}
	}
`;

const MESSAGE_SUBSCRIPTION = gql`
	subscription OnSendMessage {
		onSendMessage {
			id
			body
			createdAt
		}
	}
`;
export interface SubscriptionProps {}

const useStyles = makeStyles((theme) => ({
	root: {
		width: "800px",
		marginBottom: "60px",
	},
	grid1: {
		width: "300px",
		margin: "20px",
		marginTop: "-15px",
	},
	grid2: {
		width: "400px",
		padding: "10px",
		maxHeight: "300px",
		overflow: "scroll",
	},
	paper: {
		marginTop: theme.spacing(8),
		display: "flex",
		flexDirection: "column",
		alignItems: "center",
	},
	avatar: {
		margin: theme.spacing(1),
		backgroundColor: theme.palette.secondary.main,
	},
	form: {
		width: "100%",
		marginTop: theme.spacing(1),
	},
	submit: {
		margin: theme.spacing(3, 0, 2),
	},
	sub: {
        width: "280px",
		padding: "10px",
        overflow: "scroll"
	},
}));

const Subscription: React.FC<SubscriptionProps> = () => {
	const classes = useStyles();
	const [display, setDisplay] = useState<any[]>();
	const [input, setInput] = useState<string>("");
	const { loading, error, data: messages } = useQuery(MESSAGES);
	const [createMessage, { data }] = useMutation(CREATE_MESSAGE);
	const {
		data: onSendMessage,
		error: subError,
		loading: loadingSubscription,
	} = useSubscription(MESSAGE_SUBSCRIPTION);
	if (subError) console.log(subError);
	useEffect(() => {
		setDisplay(
			loading ? ["loading"] : error ? [error.message] : messages.messages
		);
	}, [loading, error, messages]);
	useEffect(() => {
		if (data?.createMessage?.body)
			setDisplay((state) => {
				let arr: string[] = (state && [...state]) || [""];
				arr.push(data.createMessage);
				return arr;
			});
	}, [data]);

	return (
		<>
			<h3 style={{ textAlign: "center" }}>Test Subscriptions</h3>
			<Grid container direction="row" className={classes.root}>
				<Grid container direction="column" className={classes.grid1}>
					<TextField
						variant="outlined"
						margin="normal"
						required
						fullWidth
						label="Send a mesage"
						autoFocus
						value={input}
						onChange={(event) => setInput(event.target.value)}
					/>
					<Button
						fullWidth
						variant="contained"
						color="primary"
						className={classes.submit}
						onClick={() => {
							createMessage({ variables: { body: input } });
							setInput("");
						}}
					>
						Send
					</Button>
					{!loadingSubscription && (
						<>
							<h4>this comes from subscription</h4>
							<Card className={classes.sub}>
								<span
									style={{ fontWeight: "bold" }}
								>{`${onSendMessage.onSendMessage.body}`}</span>
								<span>{"  -->  "}</span>
								<span style={{ color: "red" }}>{`${new Date(
									onSendMessage.onSendMessage.createdAt
								).toDateString()}`}</span>
							</Card>
						</>
					)}
				</Grid>
				<Card className={classes.grid2}>
					{display?.map((item: any, key: number) => (
						<p key={key}>
							{typeof item == "object" ? (
								<>
									<span
										style={{ fontWeight: "bold" }}
									>{`${item.body}`}</span>
									{"  -->  "}{" "}
									<span style={{ color: "red" }}>{`${new Date(
										item.createdAt
									).toDateString()}`}</span>
								</>
							) : (
								item
							)}
						</p>
					))}
				</Card>
			</Grid>
		</>
	);
};

export default Subscription;

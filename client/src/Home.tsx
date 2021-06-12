import { useQuery, gql } from "@apollo/client";

const Home: React.FC = () => {
	const USERS = gql`
		query Users {
			users {
				id
				email
			}
		}
	`;
	const { loading, error, data } = useQuery(USERS);
	if (loading) return <p>Loading...</p>;
	if (error) return <p>Error :(</p>;

	return data.users.map(({ id, email }: any) => (
		<div key={id}>
			<p>
				{id}: {email}
			</p>
		</div>
	));
};

export default Home;
